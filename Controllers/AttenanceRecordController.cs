using AttenanceSystemApp.DTO;
using AttenanceSystemApp.Models;
using AttenanceSystemApp.Services;
using AttenanceSystemApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AttenanceSystemApp.Controllers
{
    public class AttenanceRecordController : Controller
    {
        AttenanceRecordService _attenanceRecordService;
        DepartmentService _departmentService;
        EmployeeService _employeeService;
        private readonly UserManager<AppUser> _userManager;
        public AttenanceRecordController(AttenanceRecordService attenanceRecordService, DepartmentService departmentService,
            EmployeeService employeeService, UserManager<AppUser> userManager)
        {
            _attenanceRecordService = attenanceRecordService;
            _departmentService = departmentService;
            _employeeService = employeeService;
            _userManager = userManager;
        }
        //Ziskani Vsech oddeleni
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            bool isSupervisor = user != null && await _userManager.IsInRoleAsync(user, "supervisor");
            bool isWorker = user != null && await _userManager.IsInRoleAsync(user, "worker");

            var allDepartments = _departmentService.GetAll();
            var allEmployees = _employeeService.GetAll();

            var model = new AttenanceRecordViewModel
            {
                Departments = allDepartments,
                Employees = allEmployees,
            };

            if (isSupervisor)
            {
                
                var supervisorEmployee = allEmployees.FirstOrDefault(e => e.UserId == user.Id);
                if (supervisorEmployee != null)
                {
                    int departmentId = supervisorEmployee.DepartmentId;

                    
                    model.Departments = allDepartments
                        .Where(d => d.Id == departmentId)
                        .ToList();

                    model.DepartmentId = departmentId;

                    
                    model.Employees = allEmployees
                        .Where(e => e.DepartmentId == departmentId)
                        .ToList();
                }
            }
            else if (isWorker)
            {
                var employee = allEmployees.FirstOrDefault(e => e.UserId == user.Id);
                if (employee != null)
                {
                    int departmentId = employee.DepartmentId;

                    model.Departments = allDepartments
                        .Where(d => d.Id == departmentId)
                        .ToList();

                    model.Employees = new List<EmployeeDTO> { employee };

                    model.DepartmentId = departmentId;
                    model.EmployeeId = employee.Id;
                }
                else
                {
                    model.Departments = new List<DepartmentDTO>();
                    model.Employees = new List<EmployeeDTO>();
                }
            }
            else
            {
                model.Departments = allDepartments;
                model.Employees = model.DepartmentId > 0
                    ? allEmployees.Where(e => e.DepartmentId == model.DepartmentId).ToList()
                    : allEmployees;
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Index(AttenanceRecordViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            bool isSupervisor = user != null && await _userManager.IsInRoleAsync(user, "supervisor");
            bool isWorker = user != null && await _userManager.IsInRoleAsync(user, "worker");

            var allDepartments = _departmentService.GetAll();
            var allEmployees = _employeeService.GetAll();

            if (isSupervisor)
            {
                var supervisorEmployee = allEmployees.FirstOrDefault(e => e.UserId == user.Id);
                if (supervisorEmployee != null)
                {
                    int departmentId = supervisorEmployee.DepartmentId;

                    model.Departments = allDepartments
                        .Where(d => d.Id == departmentId)
                        .ToList();

                    model.Employees = allEmployees
                        .Where(e => e.DepartmentId == departmentId)
                        .ToList();

                    model.DepartmentId = departmentId; 
                }
                else
                {
                    
                    model.Departments = new List<DepartmentDTO>();
                    model.Employees = new List<EmployeeDTO>();
                }
            }
            else if (isWorker)
            {
                var employee = allEmployees.FirstOrDefault(e => e.UserId == user.Id);
                if (employee != null)
                {
                    int departmentId = employee.DepartmentId;

                    model.Departments = allDepartments
                        .Where(d => d.Id == departmentId)
                        .ToList();

                    model.Employees = new List<EmployeeDTO> { employee };

                    model.DepartmentId = departmentId;
                    model.EmployeeId = employee.Id;
                }
                else
                {
                    model.Departments = new List<DepartmentDTO>();
                    model.Employees = new List<EmployeeDTO>();
                }
            }
            else
            {
                model.Departments = allDepartments;
                model.Employees = model.DepartmentId > 0
                    ? allEmployees.Where(e => e.DepartmentId == model.DepartmentId).ToList()
                    : allEmployees;
            }

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
