using FluentAssertions;
using NovaCRM.Application.Features.Deals.Commands;
using NovaCRM.Application.Validators;
using NovaCRM.Domain.Enums;

namespace NovaCRM.UnitTests.Validators;

public class CreateDealValidatorTests
{
    private readonly CreateDealValidator _sut = new();

    [Fact]
    public async Task Valid_command_passes_validation()
    {
        var cmd = new CreateDealCommand(Guid.NewGuid(), "Enterprise Deal", 50000, DealStage.New, DateTime.UtcNow.AddDays(30));
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Empty_title_fails_validation()
    {
        var cmd = new CreateDealCommand(Guid.NewGuid(), "", 1000, DealStage.New, null);
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task Zero_value_fails_validation()
    {
        var cmd = new CreateDealCommand(Guid.NewGuid(), "Deal", 0, DealStage.New, null);
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Value");
    }

    [Fact]
    public async Task Negative_value_fails_validation()
    {
        var cmd = new CreateDealCommand(Guid.NewGuid(), "Deal", -100, DealStage.New, null);
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Value");
    }

    [Fact]
    public async Task Value_exceeding_cap_fails_validation()
    {
        var cmd = new CreateDealCommand(Guid.NewGuid(), "Deal", 1_000_000_001, DealStage.New, null);
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Value");
    }

    [Fact]
    public async Task Past_expected_close_date_fails_validation()
    {
        var cmd = new CreateDealCommand(Guid.NewGuid(), "Deal", 5000, DealStage.New, DateTime.UtcNow.AddDays(-1));
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ExpectedCloseDate");
    }

    [Fact]
    public async Task Null_expected_close_date_passes_validation()
    {
        var cmd = new CreateDealCommand(Guid.NewGuid(), "Deal", 5000, DealStage.New, null);
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Empty_customer_id_fails_validation()
    {
        var cmd = new CreateDealCommand(Guid.Empty, "Deal", 5000, DealStage.New, null);
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CustomerId");
    }
}
