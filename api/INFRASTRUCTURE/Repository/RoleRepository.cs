using APPLICATION.Dto.Role;
using APPLICATION.IRepository;
using AutoMapper;
using DOMAIN.Model;
using INFRASTRUCTURE.Data;

namespace INFRASTRUCTURE.Repository;

public class RoleRepository:GenericRepository<Role, RoleDto, GetRoleDto>, IRoleRepository
{
    public RoleRepository(AppDbContext context, IMapper mapper):base(context, mapper)
    {
    }
}