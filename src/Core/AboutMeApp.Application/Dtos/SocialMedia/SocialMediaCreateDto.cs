using AboutMeApp.Domain.Entities;

namespace AboutMeApp.Application.Dtos.SocialMedia;

public record SocialMediaCreateDto
{
    public string? Platform { get; set; } // LinkedIn, GitHub ve s.
    public string? Url { get; set; } // Sosial media linki

    public Guid UserProfileId { get; set; }
}
