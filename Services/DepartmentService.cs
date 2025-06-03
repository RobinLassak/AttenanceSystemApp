using AttenanceSystemApp.DTO;
using AttenanceSystemApp.Models;

namespace AttenanceSystemApp.Services
{
    public class DepartmentService
    {
        private readonly AttenanceDbContext _dbContext;
        public DepartmentService(AttenanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //Vyobrazeni vsech oddeleni
        public List<DepartmentDTO> GetAll()
        {
            var allDepartments = _dbContext.Departments.ToList();
            var departmentsDtos = new List<DepartmentDTO>();
            foreach (var department in allDepartments)
            {
                DepartmentDTO departmentDTO = ModelToDto(department);
                departmentsDtos.Add(departmentDTO);
            }
            return departmentsDtos;
        }
        //Pomocne metody
        private static DepartmentDTO ModelToDto(Department department)
        {
            return new DepartmentDTO()
            {
                Id = department.Id,
                Name = department.Name,
            };
        }
    }
}
