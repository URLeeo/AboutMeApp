namespace AboutMeApp.Application.Dtos.Identity;

public record RegisterDto
{
    public required string Name { get; set; }
    public string? Surname { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
}
