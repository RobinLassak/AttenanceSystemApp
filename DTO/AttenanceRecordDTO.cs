using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AttenanceSystemApp.DTO
{
    public class AttenanceRecordDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
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
        [ValidateNever]
        public EmployeeDTO Employee { get; set; }
        [ValidateNever]
        public string AttenanceType { get; set; }
    }
}
