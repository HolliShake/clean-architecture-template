using System.ComponentModel.DataAnnotations;
using APPLICATION.Dto.Role;
using APPLICATION.Dto.RoleAction;
using APPLICATION.Dto.User;

namespace APPLICATION.Dto.UserXAccess;

public class GetUserXAccessDto
{
    public long Id { get; set; }
    
    // Fk User
    public string UserId { get; set; }
    public GetUserDto? User { get; set; }

    // Fk Role
    public long RoleId { get; set; }
    public GetRoleDto? Role { get; set; }

    // Fk Action
    public long RoleActionId { get; set; }
    public GetRoleActionDto? RoleAction { get; set; }
}
