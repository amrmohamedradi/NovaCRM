using AutoMapper;
using FluentAssertions;
using NSubstitute;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Features.Deals.Commands;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Enums;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.UnitTests.Handlers;

public class UpdateDealCommandHandlerTests
{
    private readonly IRepository<Deal> _repo = Substitute.For<IRepository<Deal>>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly UpdateDealCommandHandler _sut;

    public UpdateDealCommandHandlerTests()
    {
        _sut = new UpdateDealCommandHandler(_repo, _mapper);
    }

    [Fact]
    public async Task Handle_updates_deal_properties_and_persists_changes()
    {
        var dealId = Guid.NewGuid();
        var deal = new Deal
        {
            Id = dealId,
            Title = "Initial deal",
            Value = 1000,
            Stage = DealStage.New
        };

        _repo.GetByIdAsync(dealId).Returns(deal);
        _mapper.Map<DealDto>(deal).Returns(new DealDto
        {
            Id = dealId,
            Title = "Updated deal",
            Value = 2500,
            Stage = DealStage.Negotiation
        });

        var command = new UpdateDealCommand(
            dealId,
            "Updated deal",
            2500,
            DealStage.Negotiation,
            new DateTime(2026, 4, 30, 0, 0, 0, DateTimeKind.Utc));

        var result = await _sut.Handle(command, CancellationToken.None);

        deal.Title.Should().Be("Updated deal");
        deal.Value.Should().Be(2500);
        deal.Stage.Should().Be(DealStage.Negotiation);
        deal.ExpectedCloseDate.Should().Be(new DateTime(2026, 4, 30, 0, 0, 0, DateTimeKind.Utc));
        _repo.Received(1).Update(deal);
        await _repo.Received(1).SaveChangesAsync();
        result.Title.Should().Be("Updated deal");
    }

    [Fact]
    public async Task Handle_throws_when_deal_does_not_exist()
    {
        var dealId = Guid.NewGuid();
        _repo.GetByIdAsync(dealId).Returns((Deal?)null);

        var action = () => _sut.Handle(
            new UpdateDealCommand(dealId, "Updated", 2500, DealStage.Won, null),
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Deal {dealId} not found.");
    }
}
