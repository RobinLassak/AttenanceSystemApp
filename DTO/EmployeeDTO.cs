using AttenanceSystemApp.Models;

namespace AttenanceSystemApp.DTO
{
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public string? FirsName { get; set; }
        public string? LastName { get; set; }
        public int DepartmentId { get; set; }
        public int HourlyRate { get; set; }
        public Department Department { get; set; }
        public string? UserId { get; set; }
        public string? UserRole { get; set; }
    }
}
