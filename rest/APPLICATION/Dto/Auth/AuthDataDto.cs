using APPLICATION.Dto.User;

namespace APPLICATION.Dto.Auth;

public class AuthDataDto:GetUserDto
{
    public bool IsGoogle { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}