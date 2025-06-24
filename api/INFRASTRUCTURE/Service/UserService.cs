using APPLICATION.Dto.User;
using APPLICATION.IRepository;
using APPLICATION.IService;
using DOMAIN.Model;

namespace INFRASTRUCTURE.Service;

public class UserService:GenericService<IUserRepository, User, UserDto, GetUserDto>, IUserService
{
    public UserService(IUserRepository repository):base(repository)
    {
    }
}