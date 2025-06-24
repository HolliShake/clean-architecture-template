
using APPLICATION.Dto.UserXAccess;
using APPLICATION.Dto.Response;
using DOMAIN.Model;

namespace APPLICATION.IService;

public interface IUserXAccessService:IGenericService<UserXAccess, UserXAccessDto, GetUserXAccessDto>
{
    /// <summary>
    /// Check if a user has access to a specific role and action
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <param name="roleName">The name of the role</param>
    /// <param name="action">The name of the action</param>
    /// <returns>True if the user has access, false otherwise</returns>
    public bool UserHasAccess(string userId, string roleName, string action);

    /// <summary>
    /// Check if a user has access to a list of roles and actions
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <param name="allowedRoles">The list of roles and actions to check</param>
    /// <returns>True if the user has access, false otherwise</returns>
    public bool UserHasAccess(string userId, string[] allowedRoles);
}
