namespace AboutMeApp.Application.Dtos.Certificate;

public record CertificateGetDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public DateTime IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string CertificateUrl { get; set; } = null!;
}
