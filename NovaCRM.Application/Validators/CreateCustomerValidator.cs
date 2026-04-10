using FluentValidation;
using NovaCRM.Application.Features.Customers.Commands;

namespace NovaCRM.Application.Validators;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MinimumLength(2).WithMessage("Full name must be at least 2 characters.")
            .MaximumLength(200).WithMessage("Full name cannot exceed 200 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(200).WithMessage("Email cannot exceed 200 characters.");

        RuleFor(x => x.Phone)
            .MaximumLength(50).WithMessage("Phone number cannot exceed 50 characters.")
            .Matches(@"^[\d\s\+\-\(\)\.]+$")
            .WithMessage("Phone number contains invalid characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.Company)
            .MaximumLength(200).WithMessage("Company name cannot exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Company));
    }
}
