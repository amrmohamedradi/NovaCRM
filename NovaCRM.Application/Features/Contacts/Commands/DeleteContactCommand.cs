using MediatR;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Contacts.Commands;
public record DeleteContactCommand(Guid Id) : IRequest<bool>;

public class DeleteContactCommandHandler(IRepository<Contact> repo)
    : IRequestHandler<DeleteContactCommand, bool>
{
    public async Task<bool> Handle(DeleteContactCommand request, CancellationToken ct)
    {
        var contact = await repo.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Contact {request.Id} not found.");

        repo.Delete(contact);
        await repo.SaveChangesAsync();
        return true;
    }
}



