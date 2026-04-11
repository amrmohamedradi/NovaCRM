using MediatR;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;

namespace NovaCRM.Application.Features.Contacts.Commands;
public record DeleteContactCommand(Guid Id) : IRequest<bool>;

public class DeleteContactCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteContactCommand, bool>
{
    public async Task<bool> Handle(DeleteContactCommand request, CancellationToken ct)
    {
        var contact = await context.Contacts.FindAsync(new object[] { request.Id }, ct)
            ?? throw new KeyNotFoundException($"Contact {request.Id} not found.");

        context.Contacts.Remove(contact);
        await context.SaveChangesAsync(ct);
        return true;
    }
}
