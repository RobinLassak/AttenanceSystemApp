using AttenanceSystemApp.DTO;
using AttenanceSystemApp.Models;


namespace AttenanceSystemApp.Services
{
    public class EmployeeService
    {
        private readonly AttenanceDbContext _dbContext;
        public EmployeeService(AttenanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //Vyobrazeni vsech zamestnancu
        public List<EmployeeDTO> GetAll()
        {
            var allEmployees = _dbContext.Employees.ToList();
            var employeesDtos = new List<EmployeeDTO>();
            foreach (var employee in allEmployees)
            {
                EmployeeDTO employeeDTO = ModelToDto(employee);
                employeesDtos.Add(employeeDTO);
            }
            return employeesDtos;
        }
        //Pomocne metody
        private EmployeeDTO ModelToDto(Employee employee)
        {
            return new EmployeeDTO()
            {
                Id = employee.Id,
                FirsName = employee.FirsName,
                LastName = employee.LastName,
                Department = employee.Department,
                DepartmentId = employee.DepartmentId,
            };
        }
    }
}
