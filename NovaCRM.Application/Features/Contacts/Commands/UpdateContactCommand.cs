using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;

namespace NovaCRM.Application.Features.Contacts.Commands;
public record UpdateContactCommand(
    Guid Id,
    string FullName,
    string Email,
    string? Phone,
    string? Position) : IRequest<ContactDto>;

public class UpdateContactCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<UpdateContactCommand, ContactDto>
{
    public async Task<ContactDto> Handle(UpdateContactCommand request, CancellationToken ct)
    {
        var contact = await context.Contacts.FindAsync(new object[] { request.Id }, ct)
            ?? throw new KeyNotFoundException($"Contact {request.Id} not found.");

        contact.FullName = request.FullName;
        contact.Email    = request.Email;
        contact.Phone    = request.Phone;
        contact.Position = request.Position;

        context.Contacts.Update(contact);
        await context.SaveChangesAsync(ct);
        return mapper.Map<ContactDto>(contact);
    }
}
