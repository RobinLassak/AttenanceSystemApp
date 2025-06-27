using AttenanceSystemApp.DTO;
using AttenanceSystemApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AttenanceSystemApp.Services
{
    public class EmployeeService
    {
        private readonly AttenanceDbContext _dbContext;
        UserManager<AppUser> _userManager;
        public EmployeeService(AttenanceDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        //Vyobrazeni vsech zamestnancu
        public List<EmployeeDTO> GetAll()
        {
            var allEmployees = _dbContext.Employees
                .Include(e => e.Department)
                .Include(e => e.User)
                .ToList();
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
            if (!string.IsNullOrEmpty(newEmployee.UserId))
            {
                var user = await _dbContext.Users.FindAsync(newEmployee.UserId);
                if (user != null)
                {
                    user.EmployeeId = employeeToSave.Id;
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
        public List<Department> GetAllDepartments()
        {
            return _dbContext.Departments.ToList();
        }
        //Editace zamestnancu
        internal async Task<EmployeeDTO> GetByIdAsync(int id)
        {
            var employeeToEdit = await _dbContext.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id);
            return ModelToDto(employeeToEdit);
        }

        internal async Task UpdateAsync(EmployeeDTO employeeDTO, int id)
        {
            _dbContext.Update(DtoToModel(employeeDTO));
            await _dbContext.SaveChangesAsync();
        }
        //Smazani zamestnance
        internal async Task DeleteAsync(int id)
        {
            var employeeToDelete = await _dbContext.Employees.FindAsync(id);
            if (employeeToDelete != null)
            {
                _dbContext.Employees.Remove(employeeToDelete);
            }
            await _dbContext.SaveChangesAsync();
        }
        //Ziskani seznamu zamestnancu z kazdeho daneho oddeleni
        public IEnumerable<EmployeeDTO> GetEmployeesByDepartmentId(int id)
        {
            var employees = _dbContext.Employees.Where(e => e.DepartmentId == id).ToList();
            return employees.Select(e => new EmployeeDTO()
            {
                Id = e.Id,
                FirsName = e.FirsName,
                LastName = e.LastName,
                HourlyRate = e.HourlyRate,
                DepartmentId = e.DepartmentId,

            }).OrderBy(e => e.LastName);
        }
        //Pomocne metody
        private Employee DtoToModel(EmployeeDTO newEmployee)
        {
            return new Employee()
            {
                Id = newEmployee.Id,
                FirsName = newEmployee.FirsName,
                LastName = newEmployee.LastName,
                HourlyRate = newEmployee.HourlyRate,
                DepartmentId = newEmployee.DepartmentId,
            };
        }
        private EmployeeDTO ModelToDto(Employee employee)
        {
            var dto = new EmployeeDTO()
            {
                Id = employee.Id,
                FirsName = employee.FirsName,
                LastName = employee.LastName,
                HourlyRate = employee.HourlyRate,
                Department = employee.Department,
                DepartmentId = employee.DepartmentId,
                UserId = employee.User?.Id,
                UserRole = null
            };
            if (employee.User != null)
            {
                var roles = _userManager.GetRolesAsync(employee.User).Result;
                dto.UserRole = roles.FirstOrDefault();
            }

            return dto;
        }
    }
}
