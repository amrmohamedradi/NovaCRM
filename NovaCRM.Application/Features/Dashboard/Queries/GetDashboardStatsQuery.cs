using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Enums;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Dashboard.Queries;
public record GetDashboardStatsQuery : IRequest<DashboardStatsDto>;

public class GetDashboardStatsQueryHandler(
    IRepository<Customer> customerRepo,
    IRepository<Deal> dealRepo,
    IRepository<Note> noteRepo,
    IRepository<Activity> activityRepo,
    IMapper mapper)
    : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var followUpCutoff = now.AddDays(7);

        
        var customers  = (await customerRepo.GetAllAsync()).ToList();
        var deals      = (await dealRepo.GetAllAsync()).ToList();
        var notes      = (await noteRepo.GetAllAsync()).ToList();
        var activities = (await activityRepo.GetAllAsync()).ToList();

        
        var wonThisMonth = deals
            .Where(d => d.Stage == DealStage.Won && d.UpdatedAt >= monthStart)
            .ToList();

        
        var upcomingFollowUps = notes
            .Count(n => n.FollowUpDate.HasValue &&
                        n.FollowUpDate.Value >= now &&
                        n.FollowUpDate.Value <= followUpCutoff);

        
        var recentActivities = activities
            .OrderByDescending(a => a.CreatedAt)
            .Take(5)
            .ToList();

        return new DashboardStatsDto
        {
            TotalCustomers     = customers.Count,
            TotalDeals         = deals.Count,
            TotalPipelineValue = deals
                .Where(d => d.Stage != DealStage.Won && d.Stage != DealStage.Lost)
                .Sum(d => d.Value),
            DealsWonThisMonth  = wonThisMonth.Count,
            RevenueThisMonth   = wonThisMonth.Sum(d => d.Value),
            UpcomingFollowUps  = upcomingFollowUps,
            RecentActivities   = mapper.Map<List<ActivityDto>>(recentActivities)
        };
    }
}



