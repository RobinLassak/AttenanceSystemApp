namespace AttenanceSystemApp.DTO
{
    public class EmployeeMonthlySummaryDTO
    {
        public string EmployeeName { get; set; } = string.Empty;
        public int TotalWorkedHours { get; set; }
        public int TotalDoctorHours { get; set; }
        public int VacationDays { get; set; }
        public int SickLeaveDays { get; set; }
    }
}
