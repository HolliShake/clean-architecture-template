using DOMAIN.Model;
using DOMAIN.Constant;
using Microsoft.EntityFrameworkCore;

namespace INFRASTRUCTURE.Seeder;

public class RoleSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = (long) RoleEnum.Admin, Name = "Admin" },
            new Role { Id = (long) RoleEnum.User , Name = "User"  }
        );
    }
}