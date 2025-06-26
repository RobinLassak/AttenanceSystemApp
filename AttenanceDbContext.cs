using AttenanceSystemApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity;

namespace AttenanceSystemApp
{
    public class AttenanceDbContext : IdentityDbContext<AppUser>
    {
        public AttenanceDbContext(DbContextOptions<AttenanceDbContext> options)
            :base(options) { }

        //Registrace jednotlivych tabulek do databaze
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<AttenanceRecord> AttenanceRecords { get; set; }
    }
}
