using FluentValidation;

namespace Api.Features.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .Length(10, 30).WithMessage("Username should be between 10 and 30 characters");

        RuleFor(x => x.Password)
            .Length(10, 20).WithMessage("Password should be between 10 and 20 characters");
    }
}