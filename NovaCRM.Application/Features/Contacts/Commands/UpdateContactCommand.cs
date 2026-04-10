using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Contacts.Commands;
public record UpdateContactCommand(
    Guid Id,
    string FullName,
    string Email,
    string? Phone,
    string? Position) : IRequest<ContactDto>;

public class UpdateContactCommandHandler(IRepository<Contact> repo, IMapper mapper)
    : IRequestHandler<UpdateContactCommand, ContactDto>
{
    public async Task<ContactDto> Handle(UpdateContactCommand request, CancellationToken ct)
    {
        var contact = await repo.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Contact {request.Id} not found.");

        contact.FullName = request.FullName;
        contact.Email    = request.Email;
        contact.Phone    = request.Phone;
        contact.Position = request.Position;

        repo.Update(contact);
        await repo.SaveChangesAsync();
        return mapper.Map<ContactDto>(contact);
    }
}
