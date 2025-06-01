
using APPLICATION.Dto.RoleAction;
using APPLICATION.Dto.Response;
using DOMAIN.Model;

namespace APPLICATION.IService;

public interface IRoleActionService:IGenericService<RoleAction, RoleActionDto, GetRoleActionDto>
{
}
