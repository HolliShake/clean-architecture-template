using System.ComponentModel.DataAnnotations;

namespace APPLICATION.Dto.UserXAccess;

public class UserXAccessDto
{    
    // Fk User
    [Required]
    public string UserId { get; set; }

    // Fk Role
    [Required]
    public long RoleId { get; set; }

    // Fk Action
    [Required]
    public long RoleActionId { get; set; }
}
