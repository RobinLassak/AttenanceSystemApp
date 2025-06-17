namespace AttenanceSystemApp.Models
{
    public enum DayType
    {
        Workday,
        Weekend,
        PublicHoliday,
    }
    public class CalendaryDay
    {
        public DateTime Date { get; set; }
        public DayType Type { get; set; }
        public string? HolidayName { get; set; }
    }
}
