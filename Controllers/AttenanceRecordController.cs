using AttenanceSystemApp.DTO;
using AttenanceSystemApp.Services;
using AttenanceSystemApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AttenanceSystemApp.Controllers
{
    public class AttenanceRecordController : Controller
    {
        AttenanceRecordService _attenanceRecordService;
        DepartmentService _departmentService;
        EmployeeService _employeeService;
        public AttenanceRecordController(AttenanceRecordService attenanceRecordService, DepartmentService departmentService,
            EmployeeService employeeService)
        {
            _attenanceRecordService = attenanceRecordService;
            _departmentService = departmentService;
            _employeeService = employeeService;
        }
        //Ziskani Vsech oddeleni
        public IActionResult Index()
        {
            var departments = _departmentService.GetAll();
            var employees = _employeeService.GetAll();
            var model = new AttenanceRecordViewModel
            {
                Departments = departments,
                Employees = employees,
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Index(AttenanceRecordViewModel model)
        {
            var departments = _departmentService.GetAll();
            var employees = model.DepartmentId > 0
                ? _employeeService.GetEmployeesByDepartmentId(model.DepartmentId).ToList()
                : _employeeService.GetAll();

            model.Departments = departments;
            model.Employees = employees;

            return View(model);
        }
        //Metoda pro filtraci pracovniku podle oddeleni
        [HttpGet]
        public JsonResult GetEmployeesByDepartment(int departmentId)
        {
            var employees = departmentId > 0
                ? _employeeService.GetEmployeesByDepartmentId(departmentId)
                : _employeeService.GetAll();

            var result = employees.Select(e => new
            {
                id = e.Id,
                name = $"{e.FirsName} {e.LastName}"
            });

            return Json(result);
        }
        //Ukladani dochazky do databaze
        [HttpPost]
        public async Task<IActionResult> RecordAttenance(AttenanceRecordDTO newRecord)
        {
            if(newRecord.EmployeeId == 0)
            {
                TempData["ErrorMessage"] = "Please select employee before record attenance";
                return RedirectToAction("Index");
            }
            try
            {
                await _attenanceRecordService.RecordAttenance(newRecord);
                TempData["SuccessMessage"] = "Attendance recorded successfully.";
            }
            catch (InvalidOperationException e)
            {

                TempData["ErrorMessage"] = e.Message;
            }
            return RedirectToAction("Index", new {EmployeeId = newRecord.EmployeeId });
        }
    }
}
