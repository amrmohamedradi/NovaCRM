using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Customers.Queries;
public record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDetailDto>;

public class GetCustomerByIdQueryHandler(IRepository<Customer> repo, IMapper mapper)
    : IRequestHandler<GetCustomerByIdQuery, CustomerDetailDto>
{
    public async Task<CustomerDetailDto> Handle(GetCustomerByIdQuery request, CancellationToken ct)
    {
        
        var customers = await repo.FindAsync(c => c.Id == request.Id);
        var customer = customers.FirstOrDefault()
            ?? throw new KeyNotFoundException($"Customer {request.Id} not found.");

        return mapper.Map<CustomerDetailDto>(customer);
    }
}



