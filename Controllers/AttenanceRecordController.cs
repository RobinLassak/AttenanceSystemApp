using Microsoft.AspNetCore.Mvc;

namespace AttenanceSystemApp.Controllers
{
    public class AttenanceRecordController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
