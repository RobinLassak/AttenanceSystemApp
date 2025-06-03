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
    }
}
