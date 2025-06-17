using AttenanceSystemApp.DTO;
using AttenanceSystemApp.Services;
using AttenanceSystemApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AttenanceSystemApp.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;
        public DepartmentController(DepartmentService departmentService, EmployeeService employeeService)
        {
            _departmentService = departmentService;
            _employeeService = employeeService;
        }
        //Zobrazeni vsech oddeleni
        [HttpGet]
        public IActionResult Index()
        {
            var allDepartments = _departmentService.GetAll();
            return View(allDepartments);
        }
        //Vytvoreni noveho oddeleni
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(DepartmentDTO newDepartment)
        {
            await _departmentService.CreateAsync(newDepartment);
            return RedirectToAction("Index");
        }
        //Editace oddeleni
        [HttpGet]
        public async Task<IActionResult> EditAsync(int id)
        {
            var departmentToEdit = await _departmentService.GetByIdAsync(id);
            return View(departmentToEdit);
        }
        [HttpPost]
        public async Task<IActionResult> EditAsync(DepartmentDTO departmentDTO, int id)
        {
            await _departmentService.UpdateAsync(departmentDTO, id);
            return RedirectToAction("Index");
        }
        //Smazani oddeleni
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _departmentService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
        //Metoda pro vypis zamestnancu oddeleni
        [HttpGet]
        public IActionResult Employees(int id)
        {
            var department = _departmentService.GetDepartmentById(id);
            if (department == null)
            {
                return RedirectToAction("Department not found");
            }

            var employees = _employeeService.GetEmployeesByDepartmentId(id);

            var viewModel = new DepartmentEmployeesViewModel
            {
                Department = department,
                Employees = employees
            };

            return View("DepartmentEmployees", viewModel);
        }
    }
}
