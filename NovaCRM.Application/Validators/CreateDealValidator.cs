using FluentValidation;
using NovaCRM.Application.Features.Deals.Commands;

namespace NovaCRM.Application.Validators;

public class CreateDealValidator : AbstractValidator<CreateDealCommand>
{
    public CreateDealValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Deal title is required.")
            .MinimumLength(2).WithMessage("Deal title must be at least 2 characters.")
            .MaximumLength(300).WithMessage("Deal title cannot exceed 300 characters.");

        RuleFor(x => x.Value)
            .GreaterThan(0).WithMessage("Deal value must be greater than 0.")
            .LessThanOrEqualTo(1_000_000_000).WithMessage("Deal value cannot exceed 1,000,000,000.");

        RuleFor(x => x.ExpectedCloseDate)
            .GreaterThan(DateTime.UtcNow.Date)
            .WithMessage("Expected close date must be a future date.")
            .When(x => x.ExpectedCloseDate.HasValue);
    }
}
