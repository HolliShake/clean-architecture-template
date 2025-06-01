using APPLICATION.Dto.UserXAccess;
using DOMAIN.Model;
using AutoMapper;

namespace APPLICATION.Mapper;

public class UserXAccessMapper : Profile
{
    public UserXAccessMapper()
    {
        CreateMap<UserXAccessDto, UserXAccess>();
        CreateMap<UserXAccess, GetUserXAccessDto>();
    }
}
