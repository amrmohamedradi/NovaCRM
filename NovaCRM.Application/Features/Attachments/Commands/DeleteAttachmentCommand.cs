using MediatR;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Attachments.Commands;

public record DeleteAttachmentCommand(Guid Id) : IRequest<bool>;

public class DeleteAttachmentCommandHandler(
    IRepository<Attachment> repo,
    IFileStorageService     fileStorage)
    : IRequestHandler<DeleteAttachmentCommand, bool>
{
    public async Task<bool> Handle(DeleteAttachmentCommand request, CancellationToken ct)
    {
        var attachment = await repo.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Attachment {request.Id} not found.");

        await fileStorage.DeleteAsync(attachment.CustomerId, attachment.StoredName, ct);

        repo.Delete(attachment);
        await repo.SaveChangesAsync();
        return true;
    }
}
