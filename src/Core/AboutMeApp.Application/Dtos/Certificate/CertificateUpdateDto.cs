namespace AboutMeApp.Application.Dtos.Certificate;

public record CertificateUpdateDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Issuer { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? CertificateUrl { get; set; }
}