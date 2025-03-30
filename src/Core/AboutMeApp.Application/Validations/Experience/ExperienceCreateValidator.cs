using AboutMeApp.Application.Dtos.Experience;
using FluentValidation;

namespace AboutMeApp.Application.Validations.Experience;

public class ExperienceCreateValidator : AbstractValidator<ExperienceCreateDto>
{
    public ExperienceCreateValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage("Company name is required.")
            .MaximumLength(100)
            .WithMessage("Company name cannot exceed 100 characters.");

        RuleFor(x => x.Position)
            .NotEmpty()
            .WithMessage("Position is required.")
            .MaximumLength(100)
            .WithMessage("Position cannot exceed 100 characters.");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Start date cannot be in the future.");

        RuleFor(x => x.Description)
            .MaximumLength(160)
            .WithMessage("Description cannot exceed 160 characters.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("End date must be after the start date.");

        RuleFor(x => x.UserProfileId)
            .NotEmpty()
            .WithMessage("User Profile Id is required.");
    }
}
