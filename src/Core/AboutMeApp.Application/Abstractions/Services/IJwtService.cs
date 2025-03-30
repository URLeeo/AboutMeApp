using System.Security.Claims;

namespace AboutMeApp.Application.Abstractions.Services;

public interface IJwtService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
}
