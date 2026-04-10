using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Enums;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Deals.Queries;

public record GetDealsPipelineQuery : IRequest<List<DealPipelineDto>>;

public class GetDealsPipelineQueryHandler(IRepository<Deal> repo)
    : IRequestHandler<GetDealsPipelineQuery, List<DealPipelineDto>>
{
    public async Task<List<DealPipelineDto>> Handle(
        GetDealsPipelineQuery request, CancellationToken ct)
    {

        var deals = await repo.ExecuteAsync(repo.Query(), ct);

        var grouped = deals
            .GroupBy(d => d.Stage)
            .Select(g => new DealPipelineDto
            {
                Stage      = g.Key,
                Count      = g.Count(),
                TotalValue = g.Sum(d => d.Value)
            })
            .ToDictionary(p => p.Stage);

        return Enum.GetValues<DealStage>()
            .Select(stage => grouped.TryGetValue(stage, out var dto)
                ? dto
                : new DealPipelineDto { Stage = stage, Count = 0, TotalValue = 0 })
            .ToList();
    }
}
