using AutoMapper;
using MediatR;
using NovaCRM.Application.Common;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Enums;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Deals.Queries;

public record GetAllDealsQuery(int Page, int PageSize, DealStage? Stage)
    : IRequest<PagedResult<DealDto>>;

public class GetAllDealsQueryHandler(IRepository<Deal> repo, IMapper mapper)
    : IRequestHandler<GetAllDealsQuery, PagedResult<DealDto>>
{
    public async Task<PagedResult<DealDto>> Handle(
        GetAllDealsQuery request, CancellationToken ct)
    {

        var query = repo.Query();

        if (request.Stage.HasValue)
            query = query.Where(d => d.Stage == request.Stage.Value);

        var total = await repo.CountAsync(query, ct);

        var items = await repo.ExecuteAsync(
            query
                .OrderByDescending(d => d.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize),
            ct);

        return new PagedResult<DealDto>
        {
            Items      = mapper.Map<List<DealDto>>(items),
            TotalCount = total,
            Page       = request.Page,
            PageSize   = request.PageSize
        };
    }
}
