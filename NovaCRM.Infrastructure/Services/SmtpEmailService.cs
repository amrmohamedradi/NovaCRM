using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using NovaCRM.Application.Interfaces;

namespace NovaCRM.Infrastructure.Services;

public class SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
    : IEmailService
{
    public async Task SendAsync(
        string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        var section = config.GetSection("EmailSettings");
        var enabled = section.GetValue<bool>("Enabled");

        if (!enabled)
        {

            logger.LogInformation(
                "[EMAIL — not sent, Enabled=false] To: {To} | Subject: {Subject}",
                to, subject);
            return;
        }

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                section["FromName"] ?? "NovaCRM",
                section["FromAddress"] ?? "noreply@novacrm.com"));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var body = new BodyBuilder { HtmlBody = htmlBody };
            message.Body = body.ToMessageBody();

            using var client = new SmtpClient();

            var useSsl = section.GetValue("EnableSsl", true);
            await client.ConnectAsync(
                section["Host"],
                section.GetValue("Port", 587),
                useSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
                ct);

            var username = section["Username"];
            var password = section["Password"];
            if (!string.IsNullOrEmpty(username))
                await client.AuthenticateAsync(username, password, ct);

            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);

            logger.LogInformation(
                "Email sent to {To} — Subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {

            logger.LogWarning(ex,
                "Failed to send email to {To} — Subject: {Subject}. " +
                "Check EmailSettings in appsettings.json.", to, subject);
        }
    }
}
