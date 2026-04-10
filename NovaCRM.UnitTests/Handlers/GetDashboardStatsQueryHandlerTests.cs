using AutoMapper;
using FluentAssertions;
using NSubstitute;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Features.Dashboard.Queries;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.UnitTests.Handlers;

public class GetDashboardStatsQueryHandlerTests
{
    private readonly IRepository<Customer> _customerRepo = Substitute.For<IRepository<Customer>>();
    private readonly IRepository<Deal> _dealRepo = Substitute.For<IRepository<Deal>>();
    private readonly IRepository<Note> _noteRepo = Substitute.For<IRepository<Note>>();
    private readonly IRepository<Activity> _activityRepo = Substitute.For<IRepository<Activity>>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly GetDashboardStatsQueryHandler _sut;

    public GetDashboardStatsQueryHandlerTests()
    {
        _sut = new GetDashboardStatsQueryHandler(
            _customerRepo,
            _dealRepo,
            _noteRepo,
            _activityRepo,
            _mapper);
    }

    [Fact]
    public async Task Handle_returns_aggregated_dashboard_metrics()
    {
        var activities = new List<Activity>
        {
            new() { Description = "Call customer" },
            new() { Description = "Send proposal" }
        };

        // NSubstitute last-match-wins: set Arg.Any catches first, then null overrides for null calls
        _dealRepo.CountWhereAsync(
                Arg.Any<System.Linq.Expressions.Expression<Func<Deal, bool>>>(),
                Arg.Any<CancellationToken>())
            .Returns(3);
        _dealRepo.SumDecimalAsync(
                Arg.Any<System.Linq.Expressions.Expression<Func<Deal, decimal>>>(),
                Arg.Any<System.Linq.Expressions.Expression<Func<Deal, bool>>>(),
                Arg.Any<CancellationToken>())
            .Returns(150000m, 45000m);

        // null setups added LAST so they win over Arg.Any when predicate is null
        _customerRepo.CountWhereAsync(null, Arg.Any<CancellationToken>()).Returns(12);
        _dealRepo.CountWhereAsync(null, Arg.Any<CancellationToken>()).Returns(8);
        _noteRepo.CountWhereAsync(
                Arg.Any<System.Linq.Expressions.Expression<Func<Note, bool>>>(),
                Arg.Any<CancellationToken>())
            .Returns(6);
        _activityRepo.Query().Returns(activities.AsQueryable());
        _activityRepo.ExecuteAsync(Arg.Any<IQueryable<Activity>>(), Arg.Any<CancellationToken>())
            .Returns(activities);
        _mapper.Map<List<ActivityDto>>(activities).Returns(
        [
            new ActivityDto { Description = "Call customer" },
            new ActivityDto { Description = "Send proposal" }
        ]);

        var result = await _sut.Handle(new GetDashboardStatsQuery(), CancellationToken.None);

        result.TotalCustomers.Should().Be(12);
        result.TotalDeals.Should().Be(8);
        result.TotalPipelineValue.Should().Be(150000m);
        result.DealsWonThisMonth.Should().Be(3);
        result.RevenueThisMonth.Should().Be(45000m);
        result.UpcomingFollowUps.Should().Be(6);
        result.RecentActivities.Should().HaveCount(2);
        result.RecentActivities.Select(x => x.Description)
            .Should().BeEquivalentTo(["Call customer", "Send proposal"]);
    }
}
