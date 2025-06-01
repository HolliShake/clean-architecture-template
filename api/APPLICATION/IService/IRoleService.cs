
using APPLICATION.Dto.Role;
using APPLICATION.Dto.Response;
using DOMAIN.Model;

namespace APPLICATION.IService;

public interface IRoleService:IGenericService<Role, RoleDto, GetRoleDto>
{
}
