using FluentValidation;
using NovaCRM.Application.Features.Auth.Commands;

namespace NovaCRM.Application.Validators;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6)
            .WithMessage("Password must be at least 6 characters.");
    }
}



