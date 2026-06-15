using Api.Features.Shared;
using FluentValidation;

namespace Api.Features.Employees;

public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.BirthDate)
            .Must(IsValidBirthDate).WithMessage("Employee should be at least 18 years old");

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
            .GreaterThanOrEqualTo(0).WithMessage("Salary can't be negative");

        RuleFor(x => x.StartDate)
            .Must(d => d.Date <= DateTime.Now.Date).WithMessage("Start date can't be in the future");

        RuleFor(x => x.PhoneNumber)
            .Length(13).WithMessage("Phone number must be 13 characters")
            .NotEmpty().WithMessage("Empty phone number");

        RuleFor(x => x.AddressInfo)
            .NotNull().WithMessage("Address must be provided");

        When(x => x.AddressInfo != null, () =>
        {
            RuleFor(x => x.AddressInfo!).SetValidator(new AddressValidator());
        });
    }

    private static bool IsValidBirthDate(DateTime birthDate) => DateTime.Today.AddYears(-18) >= birthDate.Date;
}
