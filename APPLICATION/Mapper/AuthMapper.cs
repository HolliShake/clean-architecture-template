using APPLICATION.Dto.Auth;
using AutoMapper;
using DOMAIN.Model;

namespace APPLICATION.Mapper;

public class AuthMapper:Profile
{
    public AuthMapper()
    {
        CreateMap<User, AuthDataDto>();
    }
}