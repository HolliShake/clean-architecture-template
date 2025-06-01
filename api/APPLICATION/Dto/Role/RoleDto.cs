using System.ComponentModel.DataAnnotations;

namespace APPLICATION.Dto.Role;

public class RoleDto
{
    [Required]
    public string Name { get; set; }
}
