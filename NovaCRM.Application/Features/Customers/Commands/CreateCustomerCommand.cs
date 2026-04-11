using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Enums;

namespace NovaCRM.Application.Features.Customers.Commands;
public record CreateCustomerCommand(
    string FullName,
    string Email,
    string? Phone,
    string? Company,
    CustomerStatus Status) : IRequest<CustomerDto>;

public class CreateCustomerCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        var customer = new Customer
        {
            FullName = request.FullName,
            Email    = request.Email,
            Phone    = request.Phone,
            Company  = request.Company,
            Status   = request.Status
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync(ct);

        return mapper.Map<CustomerDto>(customer);
    }
}
