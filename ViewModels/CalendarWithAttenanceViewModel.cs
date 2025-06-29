using AttenanceSystemApp.DTO;

namespace AttenanceSystemApp.ViewModels
{
    public class CalendarWithAttenanceViewModel
    {
        public int SelectedYear { get; set; }
        public int SelectedMonth { get; set; }

        public List<string> AvailableMonths { get; set; } = new();
        public List<int> AvailableYears { get; set; } = new();

        public List<CalendaryDayDTO> CalendarDays { get; set; } = new();
        public List<DateTime> Workdays { get; set; } = new();
        public int EmployeeId { get; set; }
        public List<EmployeeMonthlySummaryDTO> MonthlySummaries { get; set; } = new();
    }
}
