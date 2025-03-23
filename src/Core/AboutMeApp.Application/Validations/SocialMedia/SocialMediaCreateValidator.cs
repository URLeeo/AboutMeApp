﻿using AboutMeApp.Application.Dtos.SocialMedia;
using FluentValidation;
using AboutMeApp.Application.Validations.Utilities;

namespace AboutMeApp.Application.Validations.SocialMedia;

public class SocialMediaCreateValidator : AbstractValidator<SocialMediaCreateDto>
{
    public SocialMediaCreateValidator()
    {
        RuleFor(x => x.Platform)
            .NotEmpty()
            .WithMessage("Platform name is required.")
            .MaximumLength(100)
            .WithMessage("Platform name cannot exceed 100 characters.");

        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage("Certificate URL is required")
            .Must(url => url.IsValidUrl())
            .WithMessage("Certificate URL is not valid.");

        RuleFor(x => x.UserProfileId)
            .NotEmpty()
            .WithMessage("UserProfileId is required.");
    }
}
