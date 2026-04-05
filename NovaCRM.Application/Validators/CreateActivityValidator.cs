using FluentValidation;
using NovaCRM.Application.Features.Activities.Commands;

namespace NovaCRM.Application.Validators;

public class CreateActivityValidator : AbstractValidator<CreateActivityCommand>
{
    public CreateActivityValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.DueDate).NotEmpty();
    }
}



