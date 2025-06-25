using AttenanceSystemApp.DTO;

namespace AttenanceSystemApp.ViewModels
{
    public class AttenanceRecordViewModel
    {
        public List<DepartmentDTO> Departments { get; set; }
        public int DepartmentId { get; set; }

        public List<EmployeeDTO> Employees { get; set; }
        public int EmployeeId { get; set; }
    }
}
