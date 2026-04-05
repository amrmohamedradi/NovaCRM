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
    public async Task<List<DealPipelineDto>> Handle(GetDealsPipelineQuery request, CancellationToken ct)
    {
        var all = await repo.GetAllAsync();

        
        var pipeline = all
            .GroupBy(d => d.Stage)
            .Select(g => new DealPipelineDto
            {
                Stage      = g.Key,
                Count      = g.Count(),
                TotalValue = g.Sum(d => d.Value)
            })
            .OrderBy(p => (int)p.Stage)
            .ToList();

        
        var allStages = Enum.GetValues<DealStage>();
        foreach (var stage in allStages)
        {
            if (!pipeline.Any(p => p.Stage == stage))
                pipeline.Add(new DealPipelineDto { Stage = stage, Count = 0, TotalValue = 0 });
        }

        return pipeline.OrderBy(p => (int)p.Stage).ToList();
    }
}



