

namespace DOMAIN.Constant;

public enum RoleActionEnum
{
    READ   = 1,
    CREATE = 2,
    UPDATE = 3,
    DELETE = 4,
    ALL    = 5
}


public static class RoleActionEnumExtension
{
    public static string GetName(this RoleActionEnum action)
    {
        switch (action)
        {
            case RoleActionEnum.READ:
                return "Read";
            case RoleActionEnum.CREATE:
                return "Create";
            case RoleActionEnum.UPDATE:
                return "Update";
            case RoleActionEnum.DELETE:
                return "Delete";
            case RoleActionEnum.ALL:
                return "All";
            default:
                return "Unknown";
        }
    }
}
