using AboutMeApp.Domain.Entities.Common;

namespace AboutMeApp.Domain.Entities;

public class Education : BaseEntity
{
    public string SchoolName { get; set; } = null!;
    public string Degree { get; set; } = null!;
    public string FieldOfStudy { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; } = null!;
}
