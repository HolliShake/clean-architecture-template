using System.Security.Claims;

namespace CQI.APPLICATION.Jwt;

public abstract class JwtGenerator
{
    public static JwtAuthResult GenerateToken(IJwtAuthManager ijwAuthManager, string id, string userEmail, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Email, userEmail),
            new Claim(ClaimTypes.Role, role)
        };

        return ijwAuthManager.GenerateTokens(userEmail, claims, DateTime.Now);
    }
}