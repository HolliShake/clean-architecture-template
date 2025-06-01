
using APPLICATION.Dto.UserXAccess;
using APPLICATION.Dto.Response;
using DOMAIN.Model;

namespace APPLICATION.IService;

public interface IUserXAccessService:IGenericService<UserXAccess, UserXAccessDto, GetUserXAccessDto>
{
}
