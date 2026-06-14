using System.Data;
using FluentValidation;

namespace Api.Features.Auth;

public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator()
    {
        RuleFor(x => x.BirthDate)
            .Must(IsValidBirthDate).WithMessage("Employee should be at least 18 year old");

        RuleFor(x => x.Firstname)
            .MaximumLength(50).WithMessage("Firstname should have less than 50 characters")
            .NotEmpty().WithMessage("Empty firstname");

        RuleFor(x => x.Patronymic)
           .MaximumLength(50).WithMessage("Patronymic should have less than 50 characters").When(x => x.Patronymic != null)
           .NotEmpty().WithMessage("Empty Patronymic").When(x => x.Patronymic != null);

        RuleFor(x => x.Surname)
            .MaximumLength(50).WithMessage("Surname should have less than 50 characters")
            .NotEmpty().WithMessage("Empty Surname");

        RuleFor(x => x.Salary)
            .Must((salary) => salary >= 0).WithMessage("Can't be negative number");

        RuleFor(x => x.StartDate)
            .Must((startDate) => startDate.Date <= DateTime.Now.Date).WithMessage("Startdate can't be in the future");

        RuleFor(x => x.PhoneNumber)
            .Length(13).WithMessage("Phone number must be 13 characters")
            .NotEmpty().WithMessage("Empty phone number");

        RuleFor(x => x.AddressInfo)
            .NotNull().WithMessage("Address must be provided");

        When(x => x.AddressInfo != null, () =>
        {
            RuleFor(x => x.AddressInfo!)
                .SetValidator(new AddressValidator());
        });

        RuleFor(x => x.Username)
            .Length(10, 30).WithMessage("Username should be between 10 and 30 characters");

        RuleFor(x => x.Password)
            .Length(10, 20).WithMessage("Password should be between 10 and 20 characters");

    }
    private bool IsValidBirthDate(DateTime birthDate) => DateTime.Today.AddYears(-18) >= birthDate.Date;
}

internal class AddressValidator : AbstractValidator<Address>
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
           .Length(9).WithMessage("ZipCode should have less than 50 characters")
           .NotEmpty().WithMessage("Empty ZipCode");
    }
}