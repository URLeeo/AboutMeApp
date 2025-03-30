namespace AboutMeApp.Application.Dtos.Identity;

public record LoginDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
