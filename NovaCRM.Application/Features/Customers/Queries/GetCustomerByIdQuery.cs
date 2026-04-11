using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;

namespace NovaCRM.Application.Features.Customers.Queries;

public record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDetailDto>;

public class GetCustomerByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetCustomerByIdQuery, CustomerDetailDto>
{
    public async Task<CustomerDetailDto> Handle(GetCustomerByIdQuery request, CancellationToken ct)
    {

        var customer = await context.Customers
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .ProjectTo<CustomerDetailDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException($"Customer {request.Id} not found.");

        return customer;
    }
}
