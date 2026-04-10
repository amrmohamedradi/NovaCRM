using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Enums;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Customers.Commands;
public record UpdateCustomerCommand(
    Guid Id,
    string FullName,
    string Email,
    string? Phone,
    string? Company,
    CustomerStatus Status) : IRequest<CustomerDto>;

public class UpdateCustomerCommandHandler(IRepository<Customer> repo, IMapper mapper)
    : IRequestHandler<UpdateCustomerCommand, CustomerDto>
{
    public async Task<CustomerDto> Handle(UpdateCustomerCommand request, CancellationToken ct)
    {
        var customer = await repo.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Customer {request.Id} not found.");

        customer.FullName = request.FullName;
        customer.Email    = request.Email;
        customer.Phone    = request.Phone;
        customer.Company  = request.Company;
        customer.Status   = request.Status;

        repo.Update(customer);
        await repo.SaveChangesAsync();

        return mapper.Map<CustomerDto>(customer);
    }
}
