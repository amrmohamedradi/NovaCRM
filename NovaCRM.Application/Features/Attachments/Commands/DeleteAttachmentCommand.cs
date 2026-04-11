using MediatR;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;

namespace NovaCRM.Application.Features.Attachments.Commands;

public record DeleteAttachmentCommand(Guid Id) : IRequest<bool>;

public class DeleteAttachmentCommandHandler(
    IApplicationDbContext context,
    IFileStorageService     fileStorage)
    : IRequestHandler<DeleteAttachmentCommand, bool>
{
    public async Task<bool> Handle(DeleteAttachmentCommand request, CancellationToken ct)
    {
        var attachment = await context.Attachments.FindAsync(new object[] { request.Id }, ct)
            ?? throw new KeyNotFoundException($"Attachment {request.Id} not found.");

        await fileStorage.DeleteAsync(attachment.CustomerId, attachment.StoredName, ct);

        context.Attachments.Remove(attachment);
        await context.SaveChangesAsync(ct);
        return true;
    }
}
