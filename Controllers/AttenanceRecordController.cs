using AttenanceSystemApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AttenanceSystemApp.Controllers
{
    public class AttenanceRecordController : Controller
    {
        AttenanceRecordService _attenanceRecordService;
        public AttenanceRecordController(AttenanceRecordService attenanceRecordService)
        {
            _attenanceRecordService = attenanceRecordService;
        }

        public IActionResult Index()
        {
            return View();
        }
        
    }
}
