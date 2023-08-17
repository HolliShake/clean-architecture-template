

using Microsoft.EntityFrameworkCore;

namespace INFRASTRUCTURE.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> option) : base(option)
    {
    }
}