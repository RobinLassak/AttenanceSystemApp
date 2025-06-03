using AttenanceSystemApp.Services;
using Microsoft.AspNetCore.Mvc;

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
    }
}
