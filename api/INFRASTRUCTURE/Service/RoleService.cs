using APPLICATION.Dto.Role;
using APPLICATION.IService;
using DOMAIN.Model;
using APPLICATION.IRepository;

namespace INFRASTRUCTURE.Service;

public class RoleService:GenericService<IRoleRepository, Role, RoleDto, GetRoleDto>, IRoleService
{
    public RoleService(IRoleRepository repository):base(repository)
    {
    }
}
