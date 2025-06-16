using AttenanceSystemApp.DTO;
using AttenanceSystemApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AttenanceSystemApp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeService _employeeService;
        public EmployeeController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        //Zobrazeni vsech zamestnancu
        public IActionResult Index()
        {
            var allEmployees = _employeeService.GetAll();
            return View(allEmployees);
        }
        //Vytvoreni noveho zamestnance
        [HttpGet]
        public IActionResult Create()
        {
            var departments = _employeeService.GetAllDepartments();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(EmployeeDTO newEmployee)
        {
            await _employeeService.CreateAsync(newEmployee);
            return RedirectToAction("Index");
        }
    }
}
