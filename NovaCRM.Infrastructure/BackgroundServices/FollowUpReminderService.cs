using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NovaCRM.Application.Interfaces;
using NovaCRM.Infrastructure.Data;

namespace NovaCRM.Infrastructure.BackgroundServices;

public class FollowUpReminderService(
    IServiceScopeFactory scopeFactory,
    ILogger<FollowUpReminderService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("FollowUpReminderService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await SendRemindersAsync(stoppingToken);

            var now      = DateTime.UtcNow;
            var next8am  = now.Date.AddHours(8);
            if (next8am <= now) next8am = next8am.AddDays(1);
            var delay = next8am - now;

            logger.LogInformation(
                "FollowUpReminderService sleeping {Hours:F1}h until next run at {NextRun:u}",
                delay.TotalHours, next8am);

            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task SendRemindersAsync(CancellationToken ct)
    {
        try
        {
            await using var scope   = scopeFactory.CreateAsyncScope();
            var db           = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var now     = DateTime.UtcNow;
            var cutoff  = now.AddHours(24);

            var dueSoon = await db.Notes
                .Where(n => !n.IsDeleted
                         && n.FollowUpDate.HasValue
                         && n.FollowUpDate!.Value >= now
                         && n.FollowUpDate.Value  <= cutoff
                         && n.CreatedBy != null)
                .Include(n => n.Customer)
                .AsNoTracking()
                .ToListAsync(ct);

            if (dueSoon.Count == 0)
            {
                logger.LogInformation("FollowUpReminderService: no reminders to send today.");
                return;
            }

            foreach (var note in dueSoon)
            {
                var to      = note.CreatedBy!;
                var subject = $"[NovaCRM] Follow-up reminder: {note.Customer?.FullName ?? "Customer"}";
                var html    = BuildHtml(note.Customer?.FullName ?? "a customer",
                                        note.Content,
                                        note.FollowUpDate!.Value);

                await emailService.SendAsync(to, subject, html, ct);
            }

            logger.LogInformation(
                "FollowUpReminderService: sent {Count} reminder(s).", dueSoon.Count);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "FollowUpReminderService encountered an error.");
        }
    }

    private static string BuildHtml(
        string customerName, string noteContent, DateTime followUpDate) =>
        $"""
        <html><body style="font-family:sans-serif;color:#333">
          <h2 style="color:#5c3ee8">NovaCRM — Follow-up Reminder</h2>
          <p>You have a follow-up scheduled for <strong>{customerName}</strong>
             on <strong>{followUpDate:dddd, MMMM d yyyy 'at' HH:mm UTC}</strong>.</p>
          <blockquote style="border-left:4px solid #5c3ee8;padding-left:12px;color:#555">
            {System.Net.WebUtility.HtmlEncode(noteContent)}
          </blockquote>
          <p style="color:#999;font-size:12px">This reminder was generated automatically by NovaCRM.</p>
        </body></html>
        """;
}
