﻿using AttenanceSystemApp.Models;
using AttenanceSystemApp.Services;
using AttenanceSystemApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AttenanceSystemApp.Controllers
{
    public class CalendaryController : Controller
    {
        private readonly PublicHolidayService _publicHolidayService;
        public CalendaryController(PublicHolidayService publicHolidayService)
        {
            _publicHolidayService = publicHolidayService;
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
            var model = new CalendarViewModel
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
    }
}
