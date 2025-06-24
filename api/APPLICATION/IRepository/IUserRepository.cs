using APPLICATION.Dto.User;
using DOMAIN.Model;

namespace APPLICATION.IRepository;

public interface IUserRepository : IGenericRepository<User, UserDto, GetUserDto>
{
}