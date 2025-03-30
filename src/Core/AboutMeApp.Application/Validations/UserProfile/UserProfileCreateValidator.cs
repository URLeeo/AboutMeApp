using AboutMeApp.Application.Dtos.UserProfile;
using FluentValidation;
using AboutMeApp.Application.Validations.Utilities;

namespace AboutMeApp.Application.Validations.UserProfile;

public class UserProfileCreateValidator : AbstractValidator<UserProfileCreateDto>
{
    public UserProfileCreateValidator()
    {
        RuleFor(x => x.Bio)
            .NotEmpty()
            .WithMessage("Biography is required.")
            .MaximumLength(150)
            .WithMessage("Biography cannot exceed 150 characters.");


        RuleFor(x => x.ProfileImageUrl)
            .NotEmpty()
            .WithMessage("Profile Image URL is required")
            .Must(url => url.IsValidUrl())
            .WithMessage("Preview Image URL is not valid.");

        RuleFor(x => x.WebsiteUrl)
            .NotEmpty()
            .WithMessage("Website Url URL is required")
            .Must(url => url.IsValidUrl())
            .WithMessage("Website Url URL is not valid.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone Number is required.")
            .MaximumLength(24)
            .WithMessage("Phone Number cannot exceed 24 characters.")
            .Matches(@"^\d+$")
            .WithMessage("Phone Number must contain only digits.");


        RuleFor(x => x.Location)
            .NotEmpty()
            .WithMessage("Location is required.")
            .MaximumLength(150)
            .WithMessage("Location cannot exceed 150 characters.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User Id is required.");

        RuleFor(x => x.TemplateId)
            .NotEmpty()
            .WithMessage("Template Id is required.");
    }
}