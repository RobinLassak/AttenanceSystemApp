namespace AttenanceSystemApp.ViewModels
{
    public class CalendarViewModel
    {
        public int SelectedYear { get; set; }
        public int SelectedMonth { get; set; }

        public List<int> AvailableYears { get; set; } = new List<int>();
        public List<string> AvailableMonths { get; set; } = new List<string>();

        public List<DateTime> Workdays { get; set; } = new List<DateTime>();
    }
}
