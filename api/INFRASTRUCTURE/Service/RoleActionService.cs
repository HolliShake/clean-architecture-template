using APPLICATION.Dto.RoleAction;
using APPLICATION.IService;
using DOMAIN.Model;
using APPLICATION.IRepository;

namespace INFRASTRUCTURE.Service;

public class RoleActionService:GenericService<IRoleActionRepository, RoleAction, RoleActionDto, GetRoleActionDto>, IRoleActionService
{
    public RoleActionService(IRoleActionRepository repository):base(repository)
    {
    }
}
