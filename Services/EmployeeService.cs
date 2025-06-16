using AttenanceSystemApp.DTO;
using AttenanceSystemApp.Models;
using Microsoft.EntityFrameworkCore;

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
        //Vytvoreni noveho zamestnance
        internal async Task CreateAsync(EmployeeDTO newEmployee)
        {
            var departmentExists = await _dbContext.Departments
                .AnyAsync(d => d.Id == newEmployee.DepartmentId);
            if (!departmentExists)
            {
                throw new Exception("Zadane oddeleni neexistuje");
            }
            Employee employeeToSave = DtoToModel(newEmployee);
            _dbContext.Employees.Add(employeeToSave);
            await _dbContext.SaveChangesAsync();
        }
        public List<Department> GetAllDepartments()
        {
            return _dbContext.Departments.ToList();
        }
        //Pomocne metody
        private Employee DtoToModel(EmployeeDTO newEmployee)
        {
            return new Employee()
            {
                Id = newEmployee.Id,
                FirsName = newEmployee.FirsName,
                LastName = newEmployee.LastName,
                DepartmentId = newEmployee.DepartmentId,
            };
        }
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
