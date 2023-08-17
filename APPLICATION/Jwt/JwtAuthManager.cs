using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;


// LINK: https://github.com/Cleford7/Satisfied/blob/master/Satisfied/Common/Infrastructure/JwtAuthManager.cs
// Slightly Modified

namespace CQI.APPLICATION.Jwt;

public class JwtAuthManager : IJwtAuthManager
{
    public IImmutableDictionary<string, RefreshToken> UsersRefreshTokensReadOnlyDictionary => _usersRefreshTokens.ToImmutableDictionary();
    private readonly ConcurrentDictionary<string, RefreshToken> _usersRefreshTokens;  // can store in a database or a distributed cache
    private readonly JwtTokenConfig _tokenConfig;
   


    public JwtAuthManager(JwtTokenConfig tokenConfig)
    {
        _usersRefreshTokens = new ConcurrentDictionary<string, RefreshToken>();
        _tokenConfig = tokenConfig;
    }

    // optional: clean up expired refresh tokens
    public void RemoveExpiredRefreshTokens(DateTime now)
    {
        var expiredTokens = _usersRefreshTokens.Where(x => x.Value.ExpireAt < now).ToList();
        foreach (var expiredToken in expiredTokens)
        {
            _usersRefreshTokens.TryRemove(expiredToken.Key, out _);
        }
    }

    // can be more specific to ip, user agent, device name, etc.
    public void RemoveRefreshTokenByUserEmail(string userEmail)
    {
        var refreshTokens = _usersRefreshTokens.Where(x => x.Value.UserName == userEmail).ToList();
        foreach (var refreshToken in refreshTokens)
        {
            _usersRefreshTokens.TryRemove(refreshToken.Key, out _);
        }
    }

    public JwtAuthResult GenerateTokens(string userEmail, Claim[] claims, DateTime now)
    {
        var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);
        var jwtToken = new JwtSecurityToken(
            _tokenConfig.Issuer,
            shouldAddAudienceClaim ? _tokenConfig.Audience : string.Empty,
            claims,
            expires: now.AddYears(1),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_tokenConfig.SecurityKey), SecurityAlgorithms.HmacSha256Signature));
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        var refreshToken = new RefreshToken
        {
            UserName = userEmail,
            TokenString = GenerateRefreshTokenString(),
            // ExpireAt = now.AddDays(_jwtTokenConfig.RefreshTokenExpiration)
            ExpireAt = now.AddDays(1)
        };
        _usersRefreshTokens.AddOrUpdate(refreshToken.TokenString, refreshToken, (_, _) => refreshToken);

        return new JwtAuthResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public JwtAuthResult Refresh(string accessToken, string refreshToken, DateTime now)
    {
        var (principal, jwtToken) = DecodeJwtToken(accessToken);

        if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
        {
            throw new SecurityTokenException("Invalid token");
        }

        var userName = principal.Identity?.Name;

        if (!_usersRefreshTokens.TryGetValue(refreshToken, out var existingRefreshToken))
        {
            throw new SecurityTokenException("Invalid token");
        }
        if (existingRefreshToken.UserName != userName || existingRefreshToken.ExpireAt < now)
        {
            throw new SecurityTokenException("Invalid token");
        }

        return GenerateTokens(userName, principal.Claims.ToArray(), now); // need to recover the original claims
    }

    public (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new SecurityTokenException("Invalid token");
        }
        var principal = new JwtSecurityTokenHandler()
            .ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _tokenConfig.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_tokenConfig.SecurityKey),
                    ValidAudience = _tokenConfig.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                },
                out var validatedToken);
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        return (principal, validatedToken as JwtSecurityToken);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
    }

    private static string GenerateRefreshTokenString()
    {
        var randomNumber = new byte[32];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
