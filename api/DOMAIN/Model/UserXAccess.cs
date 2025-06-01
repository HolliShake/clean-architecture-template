

using System.Numerics;

namespace DOMAIN.Model;

public class UserXAccess
{
    public long Id { get; set; }
    
    // Fk User
    public string UserId { get; set; }
    public User User { get; set; }

    // Fk Role
    public long RoleId { get; set; }
    public Role Role { get; set; }

    // Fk Action
    public long RoleActionId { get; set; }
    public RoleAction RoleAction { get; set; }
}
