using APPLICATION.Dto.Role;
using DOMAIN.Model;

namespace APPLICATION.IRepository;

public interface IRoleRepository : IGenericRepository<Role, RoleDto, GetRoleDto>
{
}