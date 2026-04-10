using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Attachments.Commands;

public record UploadAttachmentCommand(
    Guid   CustomerId,
    string FileName,
    string ContentType,
    long   SizeBytes,
    Stream FileStream) : IRequest<AttachmentDto>;

public class UploadAttachmentCommandHandler(
    IRepository<Customer>  customerRepo,
    IRepository<Attachment> attachmentRepo,
    IFileStorageService    fileStorage)
    : IRequestHandler<UploadAttachmentCommand, AttachmentDto>
{
    public async Task<AttachmentDto> Handle(
        UploadAttachmentCommand request, CancellationToken ct)
    {

        _ = await customerRepo.GetByIdAsync(request.CustomerId)
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

        await attachmentRepo.AddAsync(attachment);
        await attachmentRepo.SaveChangesAsync();

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
