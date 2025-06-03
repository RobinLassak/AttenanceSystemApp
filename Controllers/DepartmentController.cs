using AttenanceSystemApp.DTO;
using AttenanceSystemApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AttenanceSystemApp.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly DepartmentService _departmentService;
        public DepartmentController(DepartmentService departmentService)
        {
            _departmentService = departmentService;
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
    }
}
