using AboutMeApp.Application.Dtos.Education;
using FluentValidation;

namespace AboutMeApp.Application.Validations.Education;

public class EducationUpdateValidator : AbstractValidator<EducationUpdateDto>
{
    public EducationUpdateValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Education Id is required.");

        RuleFor(x => x.SchoolName)
            .MaximumLength(100)
            .WithMessage("School name cannot exceed 100 characters.");

        RuleFor(x => x.Degree)
            .MaximumLength(100)
            .WithMessage("Degree cannot exceed 100 characters.");

        RuleFor(x => x.FieldOfStudy)
            .MaximumLength(100)
            .WithMessage("Field of study cannot exceed 100 characters.");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Start date cannot be in the future.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after the start date.")
            .When(x => x.EndDate.HasValue);

        RuleFor(x => x.UserProfileId)
            .NotEmpty()
            .WithMessage("User Profile Id is required.");
    }
}
