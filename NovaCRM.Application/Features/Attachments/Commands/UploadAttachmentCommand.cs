using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;

namespace NovaCRM.Application.Features.Attachments.Commands;

public record UploadAttachmentCommand(
    Guid   CustomerId,
    string FileName,
    string ContentType,
    long   SizeBytes,
    Stream FileStream) : IRequest<AttachmentDto>;

public class UploadAttachmentCommandHandler(
    IApplicationDbContext  context,
    IFileStorageService    fileStorage)
    : IRequestHandler<UploadAttachmentCommand, AttachmentDto>
{
    public async Task<AttachmentDto> Handle(
        UploadAttachmentCommand request, CancellationToken ct)
    {

        _ = await context.Customers.FindAsync(new object[] { request.CustomerId }, ct)
            ?? throw new KeyNotFoundException(
                $"Customer {request.CustomerId} not found.");

        var storedName = await fileStorage.SaveAsync(
            request.CustomerId,
            request.FileName,
            request.ContentType,
            request.FileStream,
            ct);

        var attachment = new Attachment
        {
            CustomerId  = request.CustomerId,
            FileName    = request.FileName,
            StoredName  = storedName,
            ContentType = request.ContentType,
            SizeBytes   = request.SizeBytes
        };

        context.Attachments.Add(attachment);
        await context.SaveChangesAsync(ct);

        return ToDto(attachment);
    }

    private static AttachmentDto ToDto(Attachment a) => new()
    {
        Id            = a.Id,
        CustomerId    = a.CustomerId,
        FileName      = a.FileName,
        ContentType   = a.ContentType,
        SizeBytes     = a.SizeBytes,
        SizeFormatted = FormatSize(a.SizeBytes),
        CreatedAt     = a.CreatedAt,
        CreatedBy     = a.CreatedBy
    };

    private static string FormatSize(long bytes) => bytes switch
    {
        < 1024                => $"{bytes} B",
        < 1024 * 1024         => $"{bytes / 1024.0:F1} KB",
        < 1024 * 1024 * 1024  => $"{bytes / (1024.0 * 1024):F1} MB",
        _                     => $"{bytes / (1024.0 * 1024 * 1024):F1} GB"
    };
}
