using AboutMeApp.Application.Dtos.Certificate;
using AboutMeApp.Application.Validations.Utilities;
using FluentValidation;

namespace AboutMeApp.Application.Validations.Certificate;

public class CertificateUpdateValidator : AbstractValidator<CertificateUpdateDto>
{
    public CertificateUpdateValidator()
    {
        RuleFor(x => x.Id)
        .NotEmpty()
        .WithMessage("Certificate Id is required.");

        RuleFor(x => x.Title)
            .MaximumLength(100)
            .WithMessage("Title cannot exceed 100 characters.")
            .When(x => x.Title is not null);

        RuleFor(x => x.Issuer)
            .MaximumLength(100)
            .WithMessage("Issuer cannot exceed 100 characters.")
            .When(x => x.Issuer is not null);

        RuleFor(x => x.IssueDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Issue date cannot be in the future.")
            .When(x => x.IssueDate.HasValue);

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(x => x.IssueDate)
            .WithMessage("Expiry date must be after the issue date.")
            .When(x => x.ExpiryDate.HasValue && x.IssueDate.HasValue);

        RuleFor(x => x.CertificateUrl)
            .Must(url => url == null || url.IsValidUrl())
            .WithMessage("Certificate URL is not valid.");
    }
}
