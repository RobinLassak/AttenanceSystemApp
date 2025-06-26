using AttenanceSystemApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AttenanceSystemApp
{
    public class AttenanceDbContext : DbContext
    {
        public AttenanceDbContext(DbContextOptions<AttenanceDbContext> options)
            :base(options) { }

        //Registrace jednotlivych tabulek do databaze
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<AttenanceRecord> AttenanceRecords { get; set; }
        public DbSet<CalendaryDay> CalendaryDays { get; set; }
    }
}
