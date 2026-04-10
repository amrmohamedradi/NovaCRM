using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Enums;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Customers.Commands;
public record CreateCustomerCommand(
    string FullName,
    string Email,
    string? Phone,
    string? Company,
    CustomerStatus Status) : IRequest<CustomerDto>;

public class CreateCustomerCommandHandler(IRepository<Customer> repo, IMapper mapper)
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

        await repo.AddAsync(customer);
        await repo.SaveChangesAsync();

        return mapper.Map<CustomerDto>(customer);
    }
}
