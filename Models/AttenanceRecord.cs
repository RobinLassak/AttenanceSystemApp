namespace AttenanceSystemApp.Models
{
    public class AttenanceRecord
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? AttenanceIn { get; set; }
        public TimeSpan? AttenanceOut { get; set; }
        public TimeSpan? DoctorIn { get; set; }
        public TimeSpan? DoctorOut { get; set; }
        public TimeSpan? SmokeIn { get; set; }
        public TimeSpan? SmokeOut { get; set; }
        public bool IsVacation { get; set; }
        public bool IsSickLeave { get; set; }
        public TimeSpan? WorkedHours { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
