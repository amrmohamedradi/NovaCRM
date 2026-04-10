using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Enums;
using NovaCRM.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace NovaCRM.Application.Features.Dashboard.Queries;

public record GetDashboardStatsQuery : IRequest<DashboardStatsDto>;

public class GetDashboardStatsQueryHandler(
    IMemoryCache           cache,
    IRepository<Customer>  customerRepo,
    IRepository<Deal>      dealRepo,
    IRepository<Note>      noteRepo,
    IRepository<Activity>  activityRepo,
    IMapper mapper)
    : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    public async Task<DashboardStatsDto> Handle(
        GetDashboardStatsQuery request, CancellationToken ct)
    {
        var now          = DateTime.UtcNow;
        var monthStart   = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var followUpEnd  = now.AddDays(7);

        const string cacheKey = "dashboard_stats_global";

        if (cache.TryGetValue(cacheKey, out DashboardStatsDto? cachedStats))
        {
            return cachedStats!;
        }

        var totalCustomers = await customerRepo.CountWhereAsync(null, ct);

        var totalDeals = await dealRepo.CountWhereAsync(null, ct);

        var totalPipelineValue = await dealRepo.SumDecimalAsync(
            d => d.Value,
            d => d.Stage != DealStage.Won && d.Stage != DealStage.Lost,
            ct);

        var dealsWonThisMonth = await dealRepo.CountWhereAsync(
            d => d.Stage == DealStage.Won && d.UpdatedAt >= monthStart,
            ct);

        var revenueThisMonth = await dealRepo.SumDecimalAsync(
            d => d.Value,
            d => d.Stage == DealStage.Won && d.UpdatedAt >= monthStart,
            ct);

        var upcomingFollowUps = await noteRepo.CountWhereAsync(
            n => n.FollowUpDate.HasValue &&
                 n.FollowUpDate!.Value >= now &&
                 n.FollowUpDate.Value  <= followUpEnd,
            ct);

        var recentActivities = await activityRepo.ExecuteAsync(
            activityRepo.Query()
                        .OrderByDescending(a => a.CreatedAt)
                        .Take(5),
            ct);

        var stats = new DashboardStatsDto
        {
            TotalCustomers     = totalCustomers,
            TotalDeals         = totalDeals,
            TotalPipelineValue = totalPipelineValue,
            DealsWonThisMonth  = dealsWonThisMonth,
            RevenueThisMonth   = revenueThisMonth,
            UpcomingFollowUps  = upcomingFollowUps,
            RecentActivities   = mapper.Map<List<ActivityDto>>(recentActivities)
        };

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        
        cache.Set(cacheKey, stats, cacheOptions);

        return stats;
    }
}
