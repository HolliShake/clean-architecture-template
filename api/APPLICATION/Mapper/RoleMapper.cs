using APPLICATION.Dto.Role;
using DOMAIN.Model;
using AutoMapper;

namespace APPLICATION.Mapper;

public class RoleMapper : Profile
{
    public RoleMapper()
    {
        CreateMap<RoleDto, Role>();
        CreateMap<Role, GetRoleDto>();
    }
}
