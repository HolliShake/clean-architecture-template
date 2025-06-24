using APPLICATION.Dto.UserXAccess;
using APPLICATION.IService;
using DOMAIN.Model;
using APPLICATION.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace INFRASTRUCTURE.Service;

public class UserXAccessService:GenericService<IUserXAccessRepository, UserXAccess, UserXAccessDto, GetUserXAccessDto>, IUserXAccessService
{
    public UserXAccessService(IUserXAccessRepository repository):base(repository)
    {
    }

    public bool UserHasAccess(string userId, string roleName, string action)
    {
        return _repository.Query($"Id={userId}|Role.Name={roleName}|RoleAction.Name={action}", "Role|RoleAction").Any();
    }

    public bool UserHasAccess(string userId, string[] allowedRoles)
    {
        // ^RoleName:RoleActionName$
        var accessRegex = new Regex(@"^(?<roleName>[^:]+):(?<actionName>[^:]+)$");
        var accessList = allowedRoles
            .Where(roleName => accessRegex.IsMatch(roleName))
            .Select(roleName => new
            {
                Role = accessRegex.Match(roleName).Groups["roleName"].Value,
                RoleAction = accessRegex.Match(roleName).Groups["actionName"].Value
            }).ToList();

        foreach (var access in accessList)
        {
            if (UserHasAccess(userId, access.Role, access.RoleAction))
            {
                return true;
            }
        }
        
        return false;
    }
}
