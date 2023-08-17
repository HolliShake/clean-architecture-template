using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CQI.APPLICATION.Jwt;

public interface IJwtAuthManager
{
    IImmutableDictionary<string, RefreshToken> UsersRefreshTokensReadOnlyDictionary { get; }
    JwtAuthResult GenerateTokens(string userEmail, Claim[] claims, DateTime now);
    JwtAuthResult Refresh(string accessToken, string refreshToken, DateTime now);
    void RemoveExpiredRefreshTokens(DateTime now);
    void RemoveRefreshTokenByUserEmail(string userEmail);
    (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token);
}