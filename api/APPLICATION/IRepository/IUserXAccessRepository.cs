using APPLICATION.Dto.UserXAccess;
using DOMAIN.Model;

namespace APPLICATION.IRepository;

public interface IUserXAccessRepository : IGenericRepository<UserXAccess, UserXAccessDto, GetUserXAccessDto>
{
}