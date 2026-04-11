using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NovaCRM.Application.Common;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Enums;

namespace NovaCRM.Application.Features.Deals.Queries;

public record GetAllDealsQuery(int Page, int PageSize, DealStage? Stage)
    : IRequest<PagedResult<DealDto>>;

public class GetAllDealsQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetAllDealsQuery, PagedResult<DealDto>>
{
    public async Task<PagedResult<DealDto>> Handle(
        GetAllDealsQuery request, CancellationToken ct)
    {

        var query = context.Deals.AsNoTracking();

        if (request.Stage.HasValue)
            query = query.Where(d => d.Stage == request.Stage.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<DealDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return new PagedResult<DealDto>
        {
            Items      = items,
            TotalCount = total,
            Page       = request.Page,
            PageSize   = request.PageSize
        };
    }
}
