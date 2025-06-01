using System.ComponentModel.DataAnnotations;

namespace APPLICATION.Dto.Auth;

public class AuthRegisterDto
{
    [EmailAddress]
    [Required]
    public string Email { get; set; }
    [Required]
    [MinLength(7)]
    public string UserName { get; set; }
    [Required]
    [MinLength(8)]
    public string Password { get; set; }
    public string? PhoneNumber { get; set; }
    /*****************************************/
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public string Role { get; set; } = "[{ \"subject\": \"Auth\", \"action\": \"read\" }]";
}