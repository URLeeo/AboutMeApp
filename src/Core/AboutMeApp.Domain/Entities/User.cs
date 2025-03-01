using Microsoft.AspNetCore.Identity;

namespace AboutMeApp.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = null!;
    public string? LastName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public UserProfile? UserProfile { get; set; }
    public ICollection<Certificate>? Certificates { get; set; }
}

