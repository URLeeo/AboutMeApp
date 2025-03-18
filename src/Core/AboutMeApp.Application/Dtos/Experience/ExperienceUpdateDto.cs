using AboutMeApp.Domain.Entities;

namespace AboutMeApp.Application.Dtos.Experience;

public class ExperienceUpdateDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = null!;
    public string Position { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
    public Guid UserProfileId { get; set; }
}
