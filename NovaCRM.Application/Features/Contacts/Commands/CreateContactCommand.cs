using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;

namespace NovaCRM.Application.Features.Contacts.Commands;
public record CreateContactCommand(
    Guid CustomerId,
    string FullName,
    string Email,
    string? Phone,
    string? Position) : IRequest<ContactDto>;

public class CreateContactCommandHandler(
    IApplicationDbContext context,
    IMapper mapper)
    : IRequestHandler<CreateContactCommand, ContactDto>
{
    public async Task<ContactDto> Handle(CreateContactCommand request, CancellationToken ct)
    {

        _ = await context.Customers.FindAsync(new object[] { request.CustomerId }, ct)
            ?? throw new KeyNotFoundException($"Customer {request.CustomerId} not found.");

        var contact = new Contact
        {
            CustomerId = request.CustomerId,
            FullName   = request.FullName,
            Email      = request.Email,
            Phone      = request.Phone,
            Position   = request.Position
        };

        context.Contacts.Add(contact);
        await context.SaveChangesAsync(ct);
        return mapper.Map<ContactDto>(contact);
    }
}
