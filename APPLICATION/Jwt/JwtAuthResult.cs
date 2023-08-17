using System.Text.Json.Serialization;

namespace CQI.APPLICATION.Jwt;

public class JwtAuthResult
{
#pragma warning disable CS8618
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }

    [JsonPropertyName("refreshToken")]
    public RefreshToken RefreshToken { get; set; }
#pragma warning restore CS8618
}