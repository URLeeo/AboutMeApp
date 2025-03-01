using AboutMeApp.Domain.Entities.Common;

namespace AboutMeApp.Domain.Entities;

public class UserProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Location { get; set; }
    public Guid? TemplateId { get; set; }
    public Template? Template { get; set; }
}