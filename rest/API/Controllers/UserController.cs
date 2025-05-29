
using APPLICATION.Dto.User;
using APPLICATION.IService;
using DOMAIN.Model;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : GenericActionController<User, IUserService, UserDto, GetUserDto>
{
    public UserController(IUserService repo):base(repo)
    {
    }
}