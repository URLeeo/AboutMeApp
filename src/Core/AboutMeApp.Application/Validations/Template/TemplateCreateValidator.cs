using AboutMeApp.Application.Dtos.Template;
using FluentValidation;
using AboutMeApp.Application.Validations.Utilities;

namespace AboutMeApp.Application.Validations.Template;

public class TemplateCreateValidator : AbstractValidator<TemplateCreateDto>
{
    public TemplateCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Template name is required.")
            .MaximumLength(100)
            .WithMessage("Template name cannot exceed 100 characters.");

        RuleFor(x => x.PreviewImageUrl)
            .NotEmpty()
            .WithMessage("Preview Image URL is required")
            .Must(url => url.IsValidUrl())
            .WithMessage("Preview Image URL is not valid.");

        RuleFor(x => x.CssFileUrl)
            .NotEmpty()
            .WithMessage("Css File Url is required.");
    }
}
