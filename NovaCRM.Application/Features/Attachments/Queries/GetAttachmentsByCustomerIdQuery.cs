using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Attachments.Queries;

public record GetAttachmentsByCustomerIdQuery(Guid CustomerId) : IRequest<List<AttachmentDto>>;

public class GetAttachmentsByCustomerIdQueryHandler(IRepository<Attachment> repo)
    : IRequestHandler<GetAttachmentsByCustomerIdQuery, List<AttachmentDto>>
{
    public async Task<List<AttachmentDto>> Handle(
        GetAttachmentsByCustomerIdQuery request, CancellationToken ct)
    {
        var attachments = await repo.ExecuteAsync(
            repo.Query()
                .Where(a => a.CustomerId == request.CustomerId)
                .OrderByDescending(a => a.CreatedAt),
            ct);

        return attachments.Select(a => new AttachmentDto
        {
            Id            = a.Id,
            CustomerId    = a.CustomerId,
            FileName      = a.FileName,
            ContentType   = a.ContentType,
            SizeBytes     = a.SizeBytes,
            SizeFormatted = FormatSize(a.SizeBytes),
            CreatedAt     = a.CreatedAt,
            CreatedBy     = a.CreatedBy
        }).ToList();
    }

    private static string FormatSize(long bytes) => bytes switch
    {
        < 1024               => $"{bytes} B",
        < 1024 * 1024        => $"{bytes / 1024.0:F1} KB",
        < 1024 * 1024 * 1024 => $"{bytes / (1024.0 * 1024):F1} MB",
        _                    => $"{bytes / (1024.0 * 1024 * 1024):F1} GB"
    };
}
