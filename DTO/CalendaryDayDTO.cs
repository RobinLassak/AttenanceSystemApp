namespace AttenanceSystemApp.DTO
{
    public class CalendaryDayDTO
    {
        public DateTime Date { get; set; }
        public string DayName { get; set; }
        public string DayType { get; set; }
        public string? HolidayName { get; set; }

        public string? AttenanceIn { get; set; }
        public string? AttenanceOut { get; set; }

        public string? DoctorIn { get; set; }
        public string? DoctorOut { get; set; }

        public string? SmokeIn { get; set; }
        public string? SmokeOut { get; set; }

        public bool IsVacation { get; set; }
        public bool IsSickLeave { get; set; }

        public string? WorkedHours { get; set; }
    }
}
