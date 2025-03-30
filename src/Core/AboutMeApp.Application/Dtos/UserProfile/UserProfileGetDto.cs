namespace AboutMeApp.Application.Dtos.UserProfile;

public class UserProfileGetDto
{
    public Guid ID { get; set; }
    public Guid UserId { get; set; }
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Location { get; set; }
    public Guid? TemplateId { get; set; }
}
