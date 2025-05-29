using System.ComponentModel.DataAnnotations;

namespace APPLICATION.Dto.Auth;

public class AuthDto
{
    [EmailAddress]
    [Required]
    public string Email { get; set; }
    [Required]
    [MinLength(8)]
    public string Password { get; set; }
}