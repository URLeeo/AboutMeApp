using AboutMeApp.Domain.Entities.Common;

namespace AboutMeApp.Domain.Entities;

public class Certificate : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public DateTime IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string CertificateUrl { get; set; } = null!;
}
