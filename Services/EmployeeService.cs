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
            // 1. Najdi existujícího zaměstnance
            var existingEmployee = await _dbContext.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (existingEmployee == null)
                throw new Exception("Zaměstnanec nenalezen");

            // 2. Aktualizuj základní údaje
            existingEmployee.FirsName = employeeDTO.FirsName;
            existingEmployee.LastName = employeeDTO.LastName;
            existingEmployee.HourlyRate = employeeDTO.HourlyRate;
            existingEmployee.DepartmentId = employeeDTO.DepartmentId;

            // 3. Pokud měl přiřazeného uživatele a ten se mění, odpoj ho
            if (existingEmployee.User != null && existingEmployee.User.Id != employeeDTO.UserId)
            {
                existingEmployee.User.EmployeeId = null;
            }

            // 4. Najdi a přiřaď nového usera
            if (!string.IsNullOrEmpty(employeeDTO.UserId))
            {
                var newUser = await _dbContext.Users.FindAsync(employeeDTO.UserId);
                if (newUser != null)
                {
                    newUser.EmployeeId = existingEmployee.Id;
                }
            }

            // 5. Ulož vše
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
            var employees = _dbContext.Employees
                .Include(e => e.Department)
                .Include(e => e.User)
                .Where(e => e.DepartmentId == id)
                .ToList();

            return employees
                .Select(e => ModelToDto(e))
                .OrderBy(e => e.LastName);
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
        public async Task<AppUser?> GetUserWithEmployeeByIdAsync(string userId)
        {
            return await _dbContext.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
