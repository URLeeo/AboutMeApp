using AboutMeApp.Domain.Entities.Common;

namespace AboutMeApp.Domain.Entities;

public class SocialMedia : BaseEntity
{
    public string? Platform { get; set; } // LinkedIn, GitHub ve s.
    public string? Url { get; set; } // Sosial media linki

    public Guid UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; } = null!;
}

