using System.ComponentModel.DataAnnotations;

namespace APPLICATION.Dto.User;

public class UserDto
{
    [EmailAddress]
    [Required]
    public string Email { get; set; }
    [Required]
    [MinLength(7)]
    public string UserName { get; set; }
    public string? PhoneNumber { get; set; }
    /*****************************************/
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Role { get; set; }
}