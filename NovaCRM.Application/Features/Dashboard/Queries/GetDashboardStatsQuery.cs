using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NovaCRM.Application.Common;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Enums;
using Microsoft.Extensions.Caching.Memory;

namespace NovaCRM.Application.Features.Dashboard.Queries;

public record GetDashboardStatsQuery : IRequest<DashboardStatsDto>;

public class GetDashboardStatsQueryHandler(
    IMemoryCache           cache,
    IApplicationDbContext  context,
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

        var totalCustomers = await context.Customers.CountAsync(ct);

        var totalDeals = await context.Deals.CountAsync(ct);

        var totalPipelineValue = await context.Deals
            .Where(d => d.Stage != DealStage.Won && d.Stage != DealStage.Lost)
            .SumAsync(d => d.Value, ct);

        var dealsWonThisMonth = await context.Deals.CountAsync(
            d => d.Stage == DealStage.Won && d.UpdatedAt >= monthStart,
            ct);

        var revenueThisMonth = await context.Deals
            .Where(d => d.Stage == DealStage.Won && d.UpdatedAt >= monthStart)
            .SumAsync(d => d.Value, ct);

        var upcomingFollowUps = await context.Notes.CountAsync(
            n => n.FollowUpDate.HasValue &&
                 n.FollowUpDate!.Value >= now &&
                 n.FollowUpDate.Value  <= followUpEnd,
            ct);

        var recentActivities = await context.Activities
            .AsNoTracking()
            .OrderByDescending(a => a.CreatedAt)
            .Take(5)
            .ProjectTo<ActivityDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        var stats = new DashboardStatsDto
        {
            TotalCustomers     = totalCustomers,
            TotalDeals         = totalDeals,
            TotalPipelineValue = totalPipelineValue,
            DealsWonThisMonth  = dealsWonThisMonth,
            RevenueThisMonth   = revenueThisMonth,
            UpcomingFollowUps  = upcomingFollowUps,
            RecentActivities   = recentActivities
        };

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        
        cache.Set(cacheKey, stats, cacheOptions);

        return stats;
    }
}
