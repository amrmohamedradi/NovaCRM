using FluentValidation;
using NovaCRM.Application.Features.Notes.Commands;

namespace NovaCRM.Application.Validators;

public class CreateNoteValidator : AbstractValidator<CreateNoteCommand>
{
    public CreateNoteValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty().MaximumLength(2000);
    }
}



