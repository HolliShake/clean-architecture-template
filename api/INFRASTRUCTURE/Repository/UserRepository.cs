using APPLICATION.Dto.User;
using APPLICATION.IRepository;
using AutoMapper;
using DOMAIN.Model;
using INFRASTRUCTURE.Data;

namespace INFRASTRUCTURE.Repository;

public class UserRepository:GenericRepository<User, UserDto, GetUserDto>, IUserRepository
{
    public UserRepository(AppDbContext context, IMapper mapper):base(context, mapper)
    {
    }
}