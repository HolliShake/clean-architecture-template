
using APPLICATION.Dto.User;
using APPLICATION.IService;
using AutoMapper;
using DOMAIN.Model;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : GenericController<User, IUserService, UserDto, GetUserDto>
{
    public UserController(IMapper mapper, IUserService repo):base(mapper, repo)
    {
    }
}