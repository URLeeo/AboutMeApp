using AboutMeApp.Application.Dtos.Certificate;
using AboutMeApp.Application.Validations.Utilities;
using FluentValidation;

namespace AboutMeApp.Application.Validations.Certificate;

public class CertificateCreateValidator : AbstractValidator<CertificateCreateDto>
{
    public CertificateCreateValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(100)
            .WithMessage("Title cannot exceed 100 characters.");

        RuleFor(x => x.Issuer)
            .NotEmpty()
            .WithMessage("Issuer is required")
            .MaximumLength(100)
            .WithMessage("Issuer cannot exceed 100 characters");

        RuleFor(x => x.IssueDate)
            .NotEmpty()
            .WithMessage("Issue date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Issue date cannot be in the future.");

        RuleFor(x => x.CertificateUrl)
            .NotEmpty()
            .WithMessage("Certificate URL is required")
            .Must(url => url.IsValidUrl())
            .WithMessage("Certificate URL is not valid.");

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(x => x.IssueDate)
            .When(x => x.ExpiryDate.HasValue)
            .WithMessage("Expiry date must be after the issue date.");
    }
}
