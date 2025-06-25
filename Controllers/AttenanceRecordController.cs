using AttenanceSystemApp.Services;
using AttenanceSystemApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AttenanceSystemApp.Controllers
{
    public class AttenanceRecordController : Controller
    {
        AttenanceRecordService _attenanceRecordService;
        DepartmentService _departmentService;
        public AttenanceRecordController(AttenanceRecordService attenanceRecordService, DepartmentService departmentService)
        {
            _attenanceRecordService = attenanceRecordService;
            _departmentService = departmentService;
        }
        //Ziskani Vsech oddeleni
        public IActionResult Index()
        {
            var departments = _departmentService.GetAll();
            var model = new AttenanceRecordViewModel
            {
                Departments = departments
            };
            return View(model);
        }
    }
}
