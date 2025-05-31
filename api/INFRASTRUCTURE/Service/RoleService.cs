
using AutoMapper;
using INFRASTRUCTURE.Data;
using APPLICATION.Dto.Role;
using APPLICATION.IService;
using DOMAIN.Model;

namespace INFRASTRUCTURE.Service;

public class RoleService:GenericService<Role, RoleDto, GetRoleDto>, IRoleService
{
    public RoleService(AppDbContext context, IMapper mapper):base(context, mapper)
    {
    }
}
