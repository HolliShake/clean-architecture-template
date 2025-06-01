
using AutoMapper;
using INFRASTRUCTURE.Data;
using APPLICATION.Dto.UserXAccess;
using APPLICATION.IService;
using DOMAIN.Model;

namespace INFRASTRUCTURE.Service;

public class UserXAccessService:GenericService<UserXAccess, UserXAccessDto, GetUserXAccessDto>, IUserXAccessService
{
    public UserXAccessService(AppDbContext context, IMapper mapper):base(context, mapper)
    {
    }
}
