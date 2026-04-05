using FluentValidation;
using NovaCRM.Application.Features.Deals.Commands;

namespace NovaCRM.Application.Validators;

public class CreateDealValidator : AbstractValidator<CreateDealCommand>
{
    public CreateDealValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Value).GreaterThan(0).WithMessage("Deal value must be greater than 0.");
    }
}



