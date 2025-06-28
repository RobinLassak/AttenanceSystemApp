using AttenanceSystemApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttenanceSystemApp.Controllers
{
    [Authorize(Roles = "Admin, Director")]
    public class RoleController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<AppUser> _userManager;
        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View(_roleManager.Roles.OrderBy(role => role.Name));
        }
        //Vytvareni nove role
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddIdentityErrors(result);
                }
            }
            return View(name);
        }
        //Mazani role
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole roleToDelete = await _roleManager.FindByIdAsync(id);
            if (roleToDelete != null)
            {
                var result = await _roleManager.DeleteAsync(roleToDelete);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddIdentityErrors(result);
                }
            }
            ModelState.AddModelError("", "Role not found");
            return View("Index", _roleManager.Roles);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetToDelete(string id)
        {
            var roleDetails = await _roleManager.FindByIdAsync(id);

            if (roleDetails == null)
            {
                return NotFound();
            }

            return View(roleDetails);
        }
        //Editace role
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            IdentityRole roleToEdit = await _roleManager.FindByIdAsync(id);

            List<AppUser> members = new List<AppUser>();
            List<AppUser> nonMembers = new List<AppUser>();

            var allUsers = await _userManager.Users.ToListAsync();

            foreach (var user in allUsers)
            {
                var isInRole = await _userManager.IsInRoleAsync(user, roleToEdit.Name);
                if (isInRole)
                {
                    members.Add(user);
                }
                else
                {
                    nonMembers.Add(user);
                }
            }

            return View(new RoleState
            {
                Members = members,
                NonMembers = nonMembers,
                Role = roleToEdit,
            });
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditAsync(RoleModification roleModification)
        {
            if (ModelState.IsValid)
            {
                foreach (string userId in roleModification.AddIds ?? new string[] { })
                {
                    AppUser userToAdd = await _userManager.FindByIdAsync(userId);
                    if (userToAdd != null)
                    {
                        IdentityResult result = await _userManager.AddToRoleAsync(userToAdd, roleModification.RoleName);
                        if (!result.Succeeded)
                        {
                            AddIdentityErrors(result);
                        }
                    }
                }
                foreach (string userId in roleModification.DeleteIds ?? new string[] { })
                {
                    AppUser userToDelete = await _userManager.FindByIdAsync(userId);
                    if (userToDelete != null)
                    {
                        IdentityResult result = await _userManager.RemoveFromRoleAsync(userToDelete, roleModification.RoleName);
                        if (!result.Succeeded)
                        {
                            AddIdentityErrors(result);
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Spatne zadana zmena, zkontroluj udaje");
            return RedirectToAction("Index");
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
