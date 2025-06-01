

using DOMAIN.Model;
using INFRASTRUCTURE.Seeder;
using Microsoft.EntityFrameworkCore;

namespace INFRASTRUCTURE.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> option) : base(option)
    {
    }

    // R
    public DbSet<Role> Roles { get; set; }

    public DbSet<RoleAction> RoleActions { get; set; }

    // U
    public DbSet<User> Users { get; set; }
    
    public DbSet<UserXAccess> UserXAccesses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {        
        // Apply seeders
        INFRASTRUCTURE.Seeder.RoleSeeder.Seed(modelBuilder);
        INFRASTRUCTURE.Seeder.RoleActionSeeder.Seed(modelBuilder);

        // Create Base Model
        base.OnModelCreating(modelBuilder);
    }
}