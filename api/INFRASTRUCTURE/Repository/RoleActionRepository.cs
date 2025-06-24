using APPLICATION.Dto.RoleAction;
using APPLICATION.IRepository;
using AutoMapper;
using DOMAIN.Model;
using INFRASTRUCTURE.Data;

namespace INFRASTRUCTURE.Repository;

public class RoleActionRepository:GenericRepository<RoleAction, RoleActionDto, GetRoleActionDto>, IRoleActionRepository
{
    public RoleActionRepository(AppDbContext context, IMapper mapper):base(context, mapper)
    {
    }
}
