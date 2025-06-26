using AttenanceSystemApp.Models;
using AttenanceSystemApp.ViewModels;
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
        //Vytvoreni noveho uzivatele
        [HttpPost]
        public async Task<IActionResult> CreateAsync(UserViewModel newUser)
        {
            if (ModelState.IsValid)
            {
                AppUser userToAdd = new AppUser()
                {
                    UserName = newUser.Name,
                    Email = newUser.Email,
                };
                IdentityResult result = await _userManager.CreateAsync(userToAdd, newUser.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(newUser);
        }
    }
}
