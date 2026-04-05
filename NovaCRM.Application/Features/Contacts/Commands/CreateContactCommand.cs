using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Contacts.Commands;
public record CreateContactCommand(
    Guid CustomerId,
    string FullName,
    string Email,
    string? Phone,
    string? Position) : IRequest<ContactDto>;

public class CreateContactCommandHandler(
    IRepository<Contact> repo,
    IRepository<Customer> customerRepo,
    IMapper mapper)
    : IRequestHandler<CreateContactCommand, ContactDto>
{
    public async Task<ContactDto> Handle(CreateContactCommand request, CancellationToken ct)
    {
        
        _ = await customerRepo.GetByIdAsync(request.CustomerId)
            ?? throw new KeyNotFoundException($"Customer {request.CustomerId} not found.");

        var contact = new Contact
        {
            CustomerId = request.CustomerId,
            FullName   = request.FullName,
            Email      = request.Email,
            Phone      = request.Phone,
            Position   = request.Position
        };

        await repo.AddAsync(contact);
        await repo.SaveChangesAsync();
        return mapper.Map<ContactDto>(contact);
    }
}



