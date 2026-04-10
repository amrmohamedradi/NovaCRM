using AutoMapper;
using MediatR;
using NovaCRM.Application.Common;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Customers.Queries;

public record GetAllCustomersQuery(int Page, int PageSize, string? Search)
    : IRequest<PagedResult<CustomerDto>>;

public class GetAllCustomersQueryHandler(IRepository<Customer> repo, IMapper mapper)
    : IRequestHandler<GetAllCustomersQuery, PagedResult<CustomerDto>>
{
    public async Task<PagedResult<CustomerDto>> Handle(
        GetAllCustomersQuery request, CancellationToken ct)
    {

        var query = repo.Query();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            query = query.Where(c =>
                c.FullName.ToLower().Contains(s) ||
                c.Email.ToLower().Contains(s));
        }

        var total = await repo.CountAsync(query, ct);

        var items = await repo.ExecuteAsync(
            query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize),
            ct);

        return new PagedResult<CustomerDto>
        {
            Items      = mapper.Map<List<CustomerDto>>(items),
            TotalCount = total,
            Page       = request.Page,
            PageSize   = request.PageSize
        };
    }
}
