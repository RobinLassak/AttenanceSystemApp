using AttenanceSystemApp.Models;
using AttenanceSystemApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AttenanceSystemApp.Controllers
{
    [Authorize(Roles = "Admin, Director")]
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
        //Zobrazeni uzivatelu
        public IActionResult Index()
        {
            return View(_userManager.Users);
        }
        //Vytvoreni noveho uzivatele
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
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
        //Editace uzivatele
        [Authorize(Roles = "Admin, Director")]
        [HttpGet]
        public async Task<IActionResult> EditAsync(string id)
        {
            var userToEdit = await _userManager.FindByIdAsync(id);
            if (userToEdit == null)
            {
                return View("NotFound");
            }
            var targetUserRoles = await _userManager.GetRolesAsync(userToEdit);

            
            var currentUser = await _userManager.GetUserAsync(User);
            var isDirector = await _userManager.IsInRoleAsync(currentUser, "Director");

            
            if (isDirector && targetUserRoles.Any(r => r == "Admin" || r == "Root"))
            {
                TempData["ErrorMessage"] = "You do not have permission to edit users Admin or Root.";
                return RedirectToAction("Index");
            }
            return View(userToEdit);
        }
        [Authorize(Roles = "Admin, Director")]
        [HttpPost]
        public async Task<IActionResult> EditAsync(string id, string username, string email, string password)
        {
            AppUser userToEdit = await _userManager.FindByIdAsync(id);
            if (userToEdit == null)
            {
                return View("NotFound");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var isDirector = await _userManager.IsInRoleAsync(currentUser, "Director");

            var targetUserRoles = await _userManager.GetRolesAsync(userToEdit);

            if (isDirector && targetUserRoles.Any(r => r == "Admin" || r == "Root"))
            {
                TempData["ErrorMessage"] = "The 'Admin' role cannot be modified by the director";
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrEmpty(username))
            {
                userToEdit.UserName = username;
            }
            if (!string.IsNullOrEmpty(email))
            {
                userToEdit.Email = email;
            }
            else
            {
                ModelState.AddModelError("", "E-mail cannot be empty");
            }
            IdentityResult validPass = null;
            if (!string.IsNullOrEmpty(password))
            {
                validPass = await _passwordValidator.ValidateAsync(_userManager, userToEdit, password);
                if (validPass.Succeeded)
                {
                    userToEdit.PasswordHash = _passwordHasher.HashPassword(userToEdit, password);
                }
                else
                {
                    AddIdentityErrors(validPass);
                }

            }
            else
            {
                ModelState.AddModelError("", "Password cannot be empty");
            }
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                if (validPass.Succeeded)
                {
                    IdentityResult result = await _userManager.UpdateAsync(userToEdit);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                        AddIdentityErrors(result);
                }
            }
            return View(userToEdit);
        }

        //Smazani uzivatele
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            AppUser userToDelete = await _userManager.FindByIdAsync(id);
            if (userToDelete != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(userToDelete);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddIdentityErrors(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "User not found");
                return View("Index", _userManager.Users);
            }
            return View(userToDelete);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetToDelete(string id)
        {
            var userDetails = await _userManager.FindByIdAsync(id);

            if (userDetails == null)
            {
                return NotFound();
            }

            return View(userDetails);
        }
        //Pomocne metody
        private void AddIdentityErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}
