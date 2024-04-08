using APPLICATION.IService;
using DOMAIN.Model;
using INFRASTRUCTURE.Data;

namespace INFRASTRUCTURE.Service;

public class UserService:GenericService<User>, IUserService
{
    public UserService(AppDbContext context):base(context)
    {
    }
}