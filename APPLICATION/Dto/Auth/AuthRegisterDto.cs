namespace APPLICATION.Dto.Auth;

public class AuthRegisterDto
{
    public string Email { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string? PhoneNumber { get; set; }
    /*****************************************/
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public string Role { get; set; } = "[{ \"subject\": \"auth\", \"action\": \"read\" }]";
}