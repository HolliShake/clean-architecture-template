using APPLICATION.Dto.UserXAccess;
using APPLICATION.IRepository;
using AutoMapper;
using DOMAIN.Model;
using INFRASTRUCTURE.Data;

namespace INFRASTRUCTURE.Repository;

public class UserXAccessRepository:GenericRepository<UserXAccess, UserXAccessDto, GetUserXAccessDto>, IUserXAccessRepository
{
    public UserXAccessRepository(AppDbContext context, IMapper mapper):base(context, mapper)
    {
    }
}