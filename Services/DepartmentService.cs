using AttenanceSystemApp.DTO;
using AttenanceSystemApp.Models;
using Microsoft.EntityFrameworkCore;

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
        //Vytvoreni noveho oddeleni
        internal async Task CreateAsync(DepartmentDTO newDepartment)
        {
            Department departmentToSave = DtoToModel(newDepartment);
            _dbContext.Departments.Add(departmentToSave);
            await _dbContext.SaveChangesAsync();
        }
        //Editace oddeleni
        internal async Task<DepartmentDTO> GetByIdAsync(int id)
        {
            var departmentToEdit = await _dbContext.Departments.FindAsync(id);
            return ModelToDto(departmentToEdit);
        }

        internal async Task UpdateAsync(DepartmentDTO departmentDTO, int id)
        {
            _dbContext.Update(DtoToModel(departmentDTO));
            await _dbContext.SaveChangesAsync();
        }
        //Smazani oddeleni
        internal async Task DeleteAsync(int id)
        {
            var departmentToDelete = await _dbContext.Departments.FindAsync(id);
            if(departmentToDelete != null)
            {
                _dbContext.Departments.Remove(departmentToDelete);
            }
            await _dbContext.SaveChangesAsync();
        }
        //Pomocne metody
        private DepartmentDTO ModelToDto(Department department)
        {
            return new DepartmentDTO()
            {
                Id = department.Id,
                Name = department.Name,
                Adress = department.Adress,
                City = department.City,
            };
        }
        private Department DtoToModel(DepartmentDTO department)
        {
            return new Department()
            {
                Id = department.Id,
                Name = department.Name,
                Adress = department.Adress,
                City = department.City,
            };
        }
    }
}
