using System.Security.Claims;

namespace APPLICATION.Jwt;

public abstract class JwtGenerator
{
    public static JwtAuthResult GenerateToken(IJwtAuthManager ijwAuthManager, string id, string userEmail)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Email, userEmail),
        };

        return ijwAuthManager.GenerateTokens(userEmail, claims, DateTime.Now);
    }
}