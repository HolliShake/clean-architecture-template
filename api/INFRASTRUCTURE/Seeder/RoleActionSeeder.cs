using DOMAIN.Model;
using DOMAIN.Constant;
using Microsoft.EntityFrameworkCore;

namespace INFRASTRUCTURE.Seeder;

public class RoleActionSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RoleAction>().HasData(
            new RoleAction { Id = (long) RoleActionEnum.READ  , Name = RoleActionEnum.READ.GetName()   },
            new RoleAction { Id = (long) RoleActionEnum.CREATE, Name = RoleActionEnum.CREATE.GetName() },
            new RoleAction { Id = (long) RoleActionEnum.UPDATE, Name = RoleActionEnum.UPDATE.GetName() },
            new RoleAction { Id = (long) RoleActionEnum.DELETE, Name = RoleActionEnum.DELETE.GetName() }
        );
    }
}
