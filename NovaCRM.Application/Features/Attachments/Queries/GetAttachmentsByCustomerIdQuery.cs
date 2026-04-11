using MediatR;
using Microsoft.EntityFrameworkCore;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;

namespace NovaCRM.Application.Features.Attachments.Queries;

public record GetAttachmentsByCustomerIdQuery(Guid CustomerId) : IRequest<List<AttachmentDto>>;

public class GetAttachmentsByCustomerIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAttachmentsByCustomerIdQuery, List<AttachmentDto>>
{
    public async Task<List<AttachmentDto>> Handle(
        GetAttachmentsByCustomerIdQuery request, CancellationToken ct)
    {
        var attachments = await context.Attachments
            .AsNoTracking()
            .Where(a => a.CustomerId == request.CustomerId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AttachmentDto
            {
                Id            = a.Id,
                CustomerId    = a.CustomerId,
                FileName      = a.FileName,
                ContentType   = a.ContentType,
                SizeBytes     = a.SizeBytes,
                CreatedAt     = a.CreatedAt,
                CreatedBy     = a.CreatedBy
            })
            .ToListAsync(ct);

        foreach (var a in attachments)
        {
            a.SizeFormatted = FormatSize(a.SizeBytes);
        }

        return attachments;
    }

    private static string FormatSize(long bytes) => bytes switch
    {
        < 1024               => $"{bytes} B",
        < 1024 * 1024        => $"{bytes / 1024.0:F1} KB",
        < 1024 * 1024 * 1024 => $"{bytes / (1024.0 * 1024):F1} MB",
        _                    => $"{bytes / (1024.0 * 1024 * 1024):F1} GB"
    };
}
