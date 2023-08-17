using System.Text;

namespace CQI.APPLICATION.Jwt;

public class JwtTokenConfig
{
    public JwtTokenConfig(string securityKey, string issuer, string audience)
    {
        SecurityKey = Encoding.UTF8.GetBytes(securityKey);
        Issuer = issuer;
        Audience = audience;
    }

    public byte[] SecurityKey { get; protected set; }
    public string Issuer { get; protected set; }
    public string Audience { get; protected set; }
}