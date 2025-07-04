﻿using AttenanceSystemApp.DTO;
using AttenanceSystemApp.Models;
using AttenanceSystemApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AttenanceSystemApp.Controllers
{
    [Authorize(Roles = "Admin, Director, Supervisor")]
    public class EmployeeController : Controller
    {
        private readonly EmployeeService _employeeService;
        UserManager<AppUser> _userManager;
        public EmployeeController(EmployeeService employeeService, UserManager<AppUser> userManager)
        {
            _employeeService = employeeService;
            _userManager = userManager;
        }
        //Zobrazeni vsech zamestnancu
        [Authorize(Roles = "Admin, Director, Supervisor")]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _employeeService.GetUserWithEmployeeByIdAsync(userId);

            //Pro supervisora
            if (user != null && await _userManager.IsInRoleAsync(user, "supervisor"))
            {
                var departmentId = user.Employee?.DepartmentId;
                if (departmentId.HasValue)
                {
                    var departmentEmployees = _employeeService.GetEmployeesByDepartmentId(departmentId.Value);
                    return View(departmentEmployees);
                }
            }

            //Pro ostatni role
            var allEmployees = _employeeService.GetAll();
            return View(allEmployees);
        }
        //Vytvoreni noveho zamestnance
        [Authorize(Roles = "Admin, Director")]
        [HttpGet]
        public IActionResult Create()
        {
            var departments = _employeeService.GetAllDepartments();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");

            var unassignedUsers = _userManager.Users
                .Where(u => u.EmployeeId == null)
                .ToList();
            ViewBag.Users = new SelectList(unassignedUsers, "Id", "UserName");
            return View();
        }
        [Authorize(Roles = "Admin, Director")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(EmployeeDTO newEmployee)
        {
            await _employeeService.CreateAsync(newEmployee);
            return RedirectToAction("Index");
        }
        //Editace zamestnancu
        [Authorize(Roles = "Admin, Director")]
        [HttpGet]
        public async Task<IActionResult> EditAsync(int id)
        {
            var employeeToEdit = await _employeeService.GetByIdAsync(id);
            var departments = _employeeService.GetAllDepartments();
            var users = _userManager.Users
                .Where(u => u.EmployeeId == null || u.EmployeeId == employeeToEdit.Id)
                .ToList();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
            ViewBag.Users = new SelectList(users, "Id", "UserName");
            return View(employeeToEdit);
        }
        [Authorize(Roles = "Admin, Director")]
        [HttpPost]
        public async Task<IActionResult> EditAsync(EmployeeDTO employeeDTO, int id)
        {
            await _employeeService.UpdateAsync(employeeDTO, id);
            return RedirectToAction("Index");
        }
        //Smazani zamestnance
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _employeeService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin, Director")]
        [HttpGet]
        public async Task<IActionResult> GetToDelete(int id)
        {
            var employeeDetails = await _employeeService.GetByIdAsync(id);
            return View(employeeDetails);
        }
    }
}
