using FluentValidation;
using NovaCRM.Application.Features.Notes.Commands;

namespace NovaCRM.Application.Validators;

public class CreateNoteValidator : AbstractValidator<CreateNoteCommand>
{
    public CreateNoteValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Note content is required.")
            .MinimumLength(2).WithMessage("Note content must be at least 2 characters.")
            .MaximumLength(2000).WithMessage("Note content cannot exceed 2000 characters.");

        RuleFor(x => x.FollowUpDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Follow-up date must be in the future.")
            .When(x => x.FollowUpDate.HasValue);
    }
}
