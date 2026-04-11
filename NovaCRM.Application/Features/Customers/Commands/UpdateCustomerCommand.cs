using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Enums;

namespace NovaCRM.Application.Features.Customers.Commands;
public record UpdateCustomerCommand(
    Guid Id,
    string FullName,
    string Email,
    string? Phone,
    string? Company,
    CustomerStatus Status) : IRequest<CustomerDto>;

public class UpdateCustomerCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<UpdateCustomerCommand, CustomerDto>
{
    public async Task<CustomerDto> Handle(UpdateCustomerCommand request, CancellationToken ct)
    {
        var customer = await context.Customers.FindAsync(new object[] { request.Id }, ct)
            ?? throw new KeyNotFoundException($"Customer {request.Id} not found.");

        customer.FullName = request.FullName;
        customer.Email    = request.Email;
        customer.Phone    = request.Phone;
        customer.Company  = request.Company;
        customer.Status   = request.Status;

        context.Customers.Update(customer);
        await context.SaveChangesAsync(ct);

        return mapper.Map<CustomerDto>(customer);
    }
}
