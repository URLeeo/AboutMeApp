using AboutMeApp.Application.Dtos.Identity;
using FluentValidation;

namespace AboutMeApp.Application.Validations.Identity;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .MinimumLength(8)
            .WithName("Email");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .WithName("Password");
    }
}
