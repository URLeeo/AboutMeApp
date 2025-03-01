using AboutMeApp.Domain.Entities.Common;

namespace AboutMeApp.Domain.Entities;

public class Experience : BaseEntity
{
    public string CompanyName { get; set; } = null!;
    public string Position { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
    public Guid UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; } = null!;
}
