using MediatR;
using Microsoft.EntityFrameworkCore;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Enums;

namespace NovaCRM.Application.Features.Deals.Queries;

public record GetDealsPipelineQuery : IRequest<List<DealPipelineDto>>;

public class GetDealsPipelineQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetDealsPipelineQuery, List<DealPipelineDto>>
{
    public async Task<List<DealPipelineDto>> Handle(
        GetDealsPipelineQuery request, CancellationToken ct)
    {

        var groupedData = await context.Deals
            .AsNoTracking()
            .GroupBy(d => d.Stage)
            .Select(g => new DealPipelineDto
            {
                Stage      = g.Key,
                Count      = g.Count(),
                TotalValue = g.Sum(d => d.Value)
            })
            .ToDictionaryAsync(p => p.Stage, ct);

        return Enum.GetValues<DealStage>()
            .Select(stage => groupedData.TryGetValue(stage, out var dto)
                ? dto
                : new DealPipelineDto { Stage = stage, Count = 0, TotalValue = 0 })
            .ToList();
    }
}
