using FluentValidation;
using NovaCRM.Application.Features.Activities.Commands;
using NovaCRM.Domain.Enums;

namespace NovaCRM.Application.Validators;

public class CreateActivityValidator : AbstractValidator<CreateActivityCommand>
{
    public CreateActivityValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage(
                $"Activity type must be one of: {string.Join(", ", Enum.GetNames<ActivityType>())}.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Activity description is required.")
            .MinimumLength(2).WithMessage("Description must be at least 2 characters.")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("Due date is required.")
            .GreaterThan(DateTime.UtcNow.AddMinutes(-5))
            .WithMessage("Due date cannot be in the past.");
    }
}
