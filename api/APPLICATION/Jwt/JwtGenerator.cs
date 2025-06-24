using System.Security.Claims;

namespace APPLICATION.Jwt;

public abstract class JwtGenerator
{
    public static JwtAuthResult GenerateToken(IJwtAuthManager ijwAuthManager, string userId, string userEmail)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, userEmail),
        };

        return ijwAuthManager.GenerateTokens(userEmail, claims, DateTime.Now);
    }
}