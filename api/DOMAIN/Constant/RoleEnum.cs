namespace DOMAIN.Constant;

public enum RoleEnum
{
    Admin = 1,
    User = 2
}

public static class RoleEnumExtension
{
    public static string GetName(this RoleEnum role)
    {
        switch (role)
        {
            case RoleEnum.Admin:
                return "Admin";
            case RoleEnum.User:
                return "User";
            default:
                return "Unknown";
        }
    }
}

