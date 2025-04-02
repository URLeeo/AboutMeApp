using AboutMeApp.Domain.Entities.Common;

namespace AboutMeApp.Domain.Entities;

public class Certificate : BaseEntity
{
    public Guid UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public DateTime IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string CertificateUrl { get; set; } = null!;
}
