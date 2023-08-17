
using Microsoft.AspNetCore.Identity;
namespace DOMAIN.Model;


public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public DateTime BirthDate { get; set; }
    public bool IsAdminVerified { get; set; }
}