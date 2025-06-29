using AttenanceSystemApp.DTO;
using AttenanceSystemApp.Models;
using AttenanceSystemApp.Services;
using AttenanceSystemApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AttenanceSystemApp.Controllers
{
    public class CalendaryController : Controller
    {
        private readonly PublicHolidayService _publicHolidayService;
        private readonly CalendaryDayService _calendaryDayService;
        private readonly EmployeeService _employeeService;
        private readonly UserManager<AppUser> _userManager;
        private readonly AttenanceReportService _attenanceReportService;
        public CalendaryController(PublicHolidayService publicHolidayService, CalendaryDayService calendaryDayService,
            UserManager<AppUser> userManager, EmployeeService employeeService, AttenanceReportService attenanceReportService)
        {
            _publicHolidayService = publicHolidayService;
            _calendaryDayService = calendaryDayService;
            _userManager = userManager;
            _employeeService = employeeService;
            _attenanceReportService = attenanceReportService;
        }

        public async Task<IActionResult> Index(int? year, int? month, string countryCode)
        {
            // Výchozí nastavení na aktuální měsíc a rok
            var selectedYear = year ?? DateTime.Now.Year;
            var selectedMonth = month ?? DateTime.Now.Month;

            // Získání pracovních dnů
            var country = string.IsNullOrEmpty(countryCode) ? "CZ" : countryCode;
            var calendarDays = await _publicHolidayService.GetMonthCalendarAsync(selectedYear, selectedMonth, country);

            // Sestavení ViewModelu
            var model = new CalendarWithAttenanceViewModel
            {
                SelectedYear = selectedYear,
                SelectedMonth = selectedMonth,
                Workdays = calendarDays
                    .Where(day => day.Type == DayType.Workday)
                    .Select(day => day.Date)
                    .ToList(),
                AvailableYears = Enumerable.Range(DateTime.Now.Year - 5, 11).ToList(),
                AvailableMonths = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
                    .Where(m => !string.IsNullOrWhiteSpace(m))
                    .ToList()
            };

            return View(model);
        }
        //Ziskani kalendare s daty dochazky vsech zamestnancu
        [HttpGet]
        public async Task<IActionResult> GetCalendarWithAttendance(int year, int month)
        {
            int? departmentId = null;
            int? employeeId = null;

            var userId = _userManager.GetUserId(User);

            var user = await _employeeService.GetUserWithEmployeeByIdAsync(userId);

            if(user != null)
            {
                if (user != null && await _userManager.IsInRoleAsync(user, "supervisor"))
                {
                    departmentId = user.Employee?.DepartmentId;
                    Console.WriteLine($"Supervisor {user.UserName} - DepartmentId: {departmentId}");
                }
                else if (await _userManager.IsInRoleAsync(user, "worker"))
                {
                    employeeId = user.Employee?.Id;
                    Console.WriteLine($"Worker {user.UserName} - EmployeeId: {employeeId}");
                }
            }

            List<CalendaryDayDTO> calendarDays;

            if (employeeId.HasValue)
            {
                // Worker
                calendarDays = await _calendaryDayService.GetOneEmployeeAttenance(year, month, employeeId);
                return View("ForOneEmployee", new CalendarWithAttenanceViewModel
                {
                    SelectedYear = year,
                    SelectedMonth = month,
                    AvailableYears = Enumerable.Range(DateTime.Now.Year - 5, 11).ToList(),
                    AvailableMonths = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
                        .Where(m => !string.IsNullOrWhiteSpace(m)).ToList(),
                    CalendarDays = calendarDays
                });
            }
            else
            {
                // Supervisor nebo admin
                calendarDays = await _calendaryDayService.GetCalendarWithAttendance(year, month, departmentId);

                return View("Index", new CalendarWithAttenanceViewModel
                {
                    SelectedYear = year,
                    SelectedMonth = month,
                    AvailableYears = Enumerable.Range(DateTime.Now.Year - 5, 11).ToList(),
                    AvailableMonths = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
                        .Where(m => !string.IsNullOrWhiteSpace(m)).ToList(),
                    CalendarDays = calendarDays,
                    MonthlySummaries = _attenanceReportService.GetMonthlySummaries(calendarDays)
                });
            }
        }
        //Ziskani kalendare s daty dochazky jednoho zamestnance
        [HttpGet]
        public async Task<IActionResult> GetOneEmployeeAttenance(int? employeeId, int year, int month)
        {
            var calendarDays = await _calendaryDayService.GetOneEmployeeAttenance(year, month, employeeId);

            var model = new CalendarWithAttenanceViewModel
            {
                SelectedYear = year,
                SelectedMonth = month,
                AvailableYears = Enumerable.Range(DateTime.Now.Year - 5, 11).ToList(),
                AvailableMonths = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
                    .Where(m => !string.IsNullOrWhiteSpace(m))
                    .ToList(),
                CalendarDays = calendarDays
            };

            return View("ForOneEmployee", model);
        }
        //Editace dochazky - kazdy kalendarni den zvlast
        [Authorize(Roles = "Admin, Director, Supervisor")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id, DateTime date)
        {
            var model = await _calendaryDayService.GetAttendanceByEmployeeAndDateAsync(id, date);
            if (model == null) return NotFound();
            return View(model);
        }
        [Authorize(Roles = "Admin, Director, Supervisor")]
        [HttpPost]
        public async Task<IActionResult> Edit(AttenanceRecordDTO model)
        {
            if (ModelState.IsValid)
            {
                await _calendaryDayService.UpdateAttendanceAsync(model);
                return RedirectToAction("GetOneEmployeeAttenance", new
                {
                    employeeId = model.EmployeeId,
                    year = model.Date.Year,
                    month = model.Date.Month
                });
            }
            return View(model);
        }
    }
}
