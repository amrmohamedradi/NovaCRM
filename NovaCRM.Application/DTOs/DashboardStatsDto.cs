namespace NovaCRM.Application.DTOs;

public class DashboardStatsDto
{
    public int TotalCustomers { get; set; }
    public int TotalDeals { get; set; }
    public decimal TotalPipelineValue { get; set; }
    public int DealsWonThisMonth { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public int UpcomingFollowUps { get; set; }
    public List<ActivityDto> RecentActivities { get; set; } = new();
}
