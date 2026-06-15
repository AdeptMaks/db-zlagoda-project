using FluentValidation;

namespace Api.Features.Shared;

public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(x => x.City)
            .MaximumLength(50).WithMessage("City should have less than 50 characters")
            .NotEmpty().WithMessage("Empty City");

        RuleFor(x => x.Street)
            .MaximumLength(50).WithMessage("Street should have less than 50 characters")
            .NotEmpty().WithMessage("Empty Street");

        RuleFor(x => x.ZipCode)
            .Length(9).WithMessage("ZipCode must be exactly 9 characters")
            .NotEmpty().WithMessage("Empty ZipCode");
    }
}
