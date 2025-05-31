

using DOMAIN.Model;
using Microsoft.EntityFrameworkCore;

namespace INFRASTRUCTURE.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> option) : base(option)
    {
    }

    public DbSet<Role> Roles { get; set; }

    public DbSet<User> Users { get; set; }
}