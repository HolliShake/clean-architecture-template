using APPLICATION.Dto.User;
using AutoMapper;
using DOMAIN.Model;

namespace APPLICATION.Mapper;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserDto, User>();
        CreateMap<User, GetUserDto>();
    }
}