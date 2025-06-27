using AttenanceSystemApp.Models;
using AttenanceSystemApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AttenanceSystemApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        UserManager<AppUser> _userManager;
        SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel();
            model.ReturnURL = returnUrl;
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        //Prihlaseni uzivatele
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                AppUser userToLogin = await _userManager.FindByNameAsync(login.UserName);
                if (userToLogin != null)
                {
                    var signInResult = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, login.Remember, false);
                    if (signInResult.Succeeded)
                    {
                        return Redirect(login.ReturnURL ?? "/");
                    }
                }
            }
            ModelState.AddModelError("", "User not found or wrong password");
            return View(login);
        }
        //Odhlaseni uzivatele
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        //Pokud user nema opravneni zobrazi se mu view AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
