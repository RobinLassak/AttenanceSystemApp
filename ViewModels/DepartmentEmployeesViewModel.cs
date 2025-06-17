using AttenanceSystemApp.DTO;

namespace AttenanceSystemApp.ViewModels
{
    public class DepartmentEmployeesViewModel
    {
        public DepartmentDTO Department { get; set; }
        public IEnumerable<EmployeeDTO> Employees { get; set; }
        public DepartmentEmployeesViewModel()
        {
            Department = new DepartmentDTO();
            Employees = new List<EmployeeDTO>();
        }
    }
}
