using Microsoft.EntityFrameworkCore;

namespace AttenanceSystemApp
{
    public class AttenanceDbContext : DbContext
    {
        public AttenanceDbContext(DbContextOptions<AttenanceDbContext> options)
            :base(options) { }
    }
}
