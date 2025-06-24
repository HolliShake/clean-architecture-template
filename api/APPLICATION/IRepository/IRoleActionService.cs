using APPLICATION.Dto.RoleAction;
using DOMAIN.Model;

namespace APPLICATION.IRepository;

public interface IRoleActionRepository : IGenericRepository<RoleAction, RoleActionDto, GetRoleActionDto>
{
}