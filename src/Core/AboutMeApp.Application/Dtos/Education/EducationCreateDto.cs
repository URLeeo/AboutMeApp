using AboutMeApp.Domain.Entities;

namespace AboutMeApp.Application.Dtos.Education;

public record EducationCreateDto
{
    public string SchoolName { get; set; } = null!;
    public string Degree { get; set; } = null!;
    public string FieldOfStudy { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid UserProfileId { get; set; }
}
