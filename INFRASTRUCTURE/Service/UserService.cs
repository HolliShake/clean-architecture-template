using APPLICATION.Dto.User;
using APPLICATION.IService;
using AutoMapper;
using DOMAIN.Model;
using INFRASTRUCTURE.Data;

namespace INFRASTRUCTURE.Service;

public class UserService:GenericService<User, UserDto, GetUserDto>, IUserService
{
    public UserService(AppDbContext context, IMapper mapper):base(context, mapper)
    {
    }
}