namespace AttenanceSystemApp.Services
{
    public class AttenanceRecordService
    {
        private readonly AttenanceDbContext _dbContext;
        public AttenanceRecordService(AttenanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
    }
}
