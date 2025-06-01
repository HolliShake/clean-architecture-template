using APPLICATION.Dto.RoleAction;
using DOMAIN.Model;
using AutoMapper;

namespace APPLICATION.Mapper;

public class RoleActionMapper : Profile
{
    public RoleActionMapper()
    {
        CreateMap<RoleActionDto, RoleAction>();
        CreateMap<RoleAction, GetRoleActionDto>();
    }
}
