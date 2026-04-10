using FluentAssertions;
using NovaCRM.Application.Features.Customers.Commands;
using NovaCRM.Application.Validators;
using NovaCRM.Domain.Enums;

namespace NovaCRM.UnitTests.Validators;

public class CreateCustomerValidatorTests
{
    private readonly CreateCustomerValidator _sut = new();

    [Fact]
    public async Task Valid_command_passes_validation()
    {
        var cmd = new CreateCustomerCommand("Alice Johnson", "alice@example.com", "+1 555-1234", "Acme", CustomerStatus.Active);
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Empty_full_name_fails_validation()
    {
        var cmd = new CreateCustomerCommand("", "alice@example.com", null, null, CustomerStatus.Lead);
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FullName");
    }

    [Fact]
    public async Task Single_char_full_name_fails_minimum_length()
    {
        var cmd = new CreateCustomerCommand("A", "alice@example.com", null, null, CustomerStatus.Lead);
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FullName");
    }

    [Fact]
    public async Task Invalid_email_fails_validation()
    {
        var cmd = new CreateCustomerCommand("Alice", "not-an-email", null, null, CustomerStatus.Active);
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task Empty_email_fails_validation()
    {
        var cmd = new CreateCustomerCommand("Alice", "", null, null, CustomerStatus.Active);
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task Phone_with_letters_fails_validation()
    {
        var cmd = new CreateCustomerCommand("Alice", "alice@example.com", "ABCDEFG", null, CustomerStatus.Active);
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Phone");
    }

    [Fact]
    public async Task Null_phone_passes_validation()
    {
        var cmd = new CreateCustomerCommand("Alice", "alice@example.com", null, null, CustomerStatus.Active);
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Full_name_exceeding_200_chars_fails_validation()
    {
        var cmd = new CreateCustomerCommand(new string('A', 201), "alice@example.com", null, null, CustomerStatus.Active);
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FullName");
    }
}
