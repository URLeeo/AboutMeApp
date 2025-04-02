namespace AboutMeApp.Application.Dtos.Identity;

public record VerifyUserDto
{
    public required string Email { get; set; }
    public required string Otp { get; set; }
}
