using AttenanceSystemApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AttenanceSystemApp.Controllers
{
    public class UserController : Controller
    {
        UserManager<AppUser> _userManager;
        IPasswordHasher<AppUser> _passwordHasher;
        IPasswordValidator<AppUser> _passwordValidator;
        public UserController(UserManager<AppUser> userManager, IPasswordHasher<AppUser> passwordHasher,
            IPasswordValidator<AppUser> passwordValidator)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _passwordValidator = passwordValidator;
        }
        public IActionResult Index()
        {
            return View(_userManager.Users);
        }
    }
}
