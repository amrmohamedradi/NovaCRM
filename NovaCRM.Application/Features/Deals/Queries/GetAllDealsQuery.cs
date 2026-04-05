using AutoMapper;
using MediatR;
using NovaCRM.Application.Common;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Enums;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Deals.Queries;
public record GetAllDealsQuery(int Page, int PageSize, DealStage? Stage) : IRequest<PagedResult<DealDto>>;

public class GetAllDealsQueryHandler(IRepository<Deal> repo, IMapper mapper)
    : IRequestHandler<GetAllDealsQuery, PagedResult<DealDto>>
{
    public async Task<PagedResult<DealDto>> Handle(GetAllDealsQuery request, CancellationToken ct)
    {
        var all = await repo.GetAllAsync();
        var query = all.AsQueryable();

        if (request.Stage.HasValue)
            query = query.Where(d => d.Stage == request.Stage.Value);

        var ordered = query.OrderByDescending(d => d.CreatedAt).ToList();
        var total = ordered.Count;
        var items = ordered
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedResult<DealDto>
        {
            Items = mapper.Map<List<DealDto>>(items),
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}



