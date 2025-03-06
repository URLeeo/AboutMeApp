using AboutMeApp.Domain.Entities;

namespace AboutMeApp.Application.Dtos.Education;

public class EducationGetDto
{
    public Guid Id { get; set; }
    public string SchoolName { get; set; } = null!;
    public string Degree { get; set; } = null!;
    public string FieldOfStudy { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid UserProfileId { get; set; }
}
