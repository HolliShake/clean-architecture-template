
using AutoMapper;
using INFRASTRUCTURE.Data;
using APPLICATION.Dto.RoleAction;
using APPLICATION.IService;
using DOMAIN.Model;

namespace INFRASTRUCTURE.Service;

public class RoleActionService:GenericService<RoleAction, RoleActionDto, GetRoleActionDto>, IRoleActionService
{
    public RoleActionService(AppDbContext context, IMapper mapper):base(context, mapper)
    {
    }
}
