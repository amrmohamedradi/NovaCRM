using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NovaCRM.Application.Common;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;

namespace NovaCRM.Application.Features.Customers.Queries;

public record GetAllCustomersQuery(int Page, int PageSize, string? Search)
    : IRequest<PagedResult<CustomerDto>>;

public class GetAllCustomersQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetAllCustomersQuery, PagedResult<CustomerDto>>
{
    public async Task<PagedResult<CustomerDto>> Handle(
        GetAllCustomersQuery request, CancellationToken ct)
    {

        var query = context.Customers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            query = query.Where(c =>
                c.FullName.ToLower().Contains(s) ||
                c.Email.ToLower().Contains(s));
        }

        var totalCount = await query.CountAsync(ct);

        var pagedItems = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<CustomerDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return new PagedResult<CustomerDto>
        {
            Items      = pagedItems,
            TotalCount = totalCount,
            Page       = request.Page,
            PageSize   = request.PageSize
        };
    }
}
