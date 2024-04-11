
using Microsoft.AspNetCore.Identity;
namespace DOMAIN.Model;


/*
 * IdentifyUser docs
 * https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.entityframeworkcore.identityuser?view=aspnetcore-1.1
 */
public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public DateTime BirthDate { get; set; }
    public string Role { get; set; }
}