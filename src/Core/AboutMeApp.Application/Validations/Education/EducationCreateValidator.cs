using AboutMeApp.Application.Dtos.Education;
using FluentValidation;

namespace AboutMeApp.Application.Validations.Education;

public class EducationCreateValidator : AbstractValidator<EducationCreateDto>
{
    public EducationCreateValidator()
    {
        RuleFor(x => x.SchoolName)
            .NotEmpty()
            .WithMessage("School name is required.")
            .MaximumLength(100)
            .WithMessage("School name cannot exceed 100 characters.");

        RuleFor(x => x.Degree)
            .NotEmpty()
            .WithMessage("Degree is required.")
            .MaximumLength(100)
            .WithMessage("Degree cannot exceed 100 characters.");

        RuleFor(x => x.FieldOfStudy)
            .NotEmpty()
            .WithMessage("Field of study is required.")
            .MaximumLength(100)
            .WithMessage("Field of study cannot exceed 100 characters.");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Start date cannot be in the future.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("End date must be after the start date.");

        RuleFor(x => x.UserProfileId)
            .NotEmpty()
            .WithMessage("User Profile Id is required.");
    }
}
