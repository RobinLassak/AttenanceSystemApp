using AttenanceSystemApp.Models;
using AttenanceSystemApp.Services;
using AttenanceSystemApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AttenanceSystemApp.Controllers
{
    public class CalendaryController : Controller
    {
        private readonly PublicHolidayService _publicHolidayService;
        private readonly CalendaryDayService _calendaryDayService;
        public CalendaryController(PublicHolidayService publicHolidayService, CalendaryDayService calendaryDayService)
        {
            _publicHolidayService = publicHolidayService;
            _calendaryDayService = calendaryDayService;
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
            var calendarDays = await _calendaryDayService.GetCalendarWithAttendance(year, month);
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
            return View("Index", model);
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
    }
}
