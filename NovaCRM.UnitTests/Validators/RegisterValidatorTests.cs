using FluentAssertions;
using NovaCRM.Application.Features.Auth.Commands;
using NovaCRM.Application.Validators;

namespace NovaCRM.UnitTests.Validators;

public class RegisterValidatorTests
{
    private readonly RegisterValidator _sut = new();

    [Fact]
    public async Task Valid_command_passes_validation()
    {
        var cmd = new RegisterCommand("Alice Johnson", "alice@example.com", "Secret1");
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Empty_email_fails_validation()
    {
        var cmd = new RegisterCommand("Alice", "", "Secret1");
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task Invalid_email_fails_validation()
    {
        var cmd = new RegisterCommand("Alice", "not-an-email", "Secret1");
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task Password_too_short_fails_validation()
    {
        var cmd = new RegisterCommand("Alice", "alice@example.com", "Ab1");
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public async Task Password_missing_uppercase_fails_validation()
    {
        var cmd = new RegisterCommand("Alice", "alice@example.com", "secret1");
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public async Task Password_missing_digit_fails_validation()
    {
        var cmd = new RegisterCommand("Alice", "alice@example.com", "Secrets");
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public async Task Empty_full_name_fails_validation()
    {
        var cmd = new RegisterCommand("", "alice@example.com", "Secret1");
        var result = await _sut.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FullName");
    }
}
