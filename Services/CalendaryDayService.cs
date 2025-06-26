namespace AttenanceSystemApp.Services
{
    public class CalendaryDayService
    {
        AttenanceDbContext _dbContext;
        public CalendaryDayService(AttenanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
