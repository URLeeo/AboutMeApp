namespace AboutMeApp.Application.Dtos.Certificate;

public record CertificateCreateDto
{
    public Guid UserProfileId { get; set; }
    public string Title { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public DateTime IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string CertificateUrl { get; set; } = null!;
}