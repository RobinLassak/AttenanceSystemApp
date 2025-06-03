namespace AttenanceSystemApp.Services
{
    public class DepartmentService
    {
        private readonly AttenanceDbContext _dbContext;
        public DepartmentService(AttenanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
