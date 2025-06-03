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
        [HttpGet]
        public IActionResult Index()
        {
            var allDepartments = _departmentService.GetAll();
            return View(allDepartments);
        }
    }
}
