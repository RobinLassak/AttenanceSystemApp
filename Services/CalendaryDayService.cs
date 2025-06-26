using AttenanceSystemApp.DTO;
using AttenanceSystemApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttenanceSystemApp.Services
{
    public class CalendaryDayService
    {
        private readonly AttenanceDbContext _dbContext;
        private readonly PublicHolidayService _publicHolidayService;
        public CalendaryDayService(AttenanceDbContext dbContext, PublicHolidayService publicHolidayService)
        {
            _dbContext = dbContext;
            _publicHolidayService = publicHolidayService;
        }
        //Ziskani kalendare s daty dochazky vsech zamestnancu
        [HttpGet]
        public async Task<List<CalendaryDayDTO>> GetCalendarWithAttendance(int year, int month)
        {
            var daysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                .Select(day => new DateTime(year, month, day))
                .ToList();

            var calendarDays = await _publicHolidayService.GetMonthCalendarAsync(year, month, "CZ");
            var employees = await _dbContext.Employees.ToListAsync();
            var records = await _dbContext.AttenanceRecords
                .Where(r => r.Date.Year == year && r.Date.Month == month)
                .ToListAsync();

            var result = new List<CalendaryDayDTO>();

            foreach (var date in daysInMonth)
            {
                var holiday = calendarDays.FirstOrDefault(cd => cd.Date.Date == date.Date);

                foreach (var emp in employees)
                {
                    var record = records.FirstOrDefault(r => r.EmployeeId == emp.Id && r.Date.Date == date.Date);

                    result.Add(new CalendaryDayDTO
                    {
                        Date = date,
                        DayType = holiday?.Type.ToString() ?? DayType.Workday.ToString(),
                        HolidayName = holiday?.HolidayName,
                        EmployeeName = emp.LastName,

                        AttenanceIn = record?.AttenanceIn?.ToString(@"hh\:mm"),
                        AttenanceOut = record?.AttenanceOut?.ToString(@"hh\:mm"),
                        DoctorIn = record?.DoctorIn?.ToString(@"hh\:mm"),
                        DoctorOut = record?.DoctorOut?.ToString(@"hh\:mm"),
                        SmokeIn = record?.SmokeIn?.ToString(@"hh\:mm"),
                        SmokeOut = record?.SmokeOut?.ToString(@"hh\:mm"),
                        IsVacation = record?.IsVacation ?? false,
                        IsSickLeave = record?.IsSickLeave ?? false
                    });
                }
            }

            return result;
        }
        //Ziskani kalendare s daty dochazky jednoho zamestnance
        [HttpGet]
        internal async Task<List<CalendaryDayDTO>> GetOneEmployeeAttenance(int year, int month, int? employeeId)
        {
            if (employeeId == null || employeeId <= 0)
            {
                return new List<CalendaryDayDTO>();
            }

            var daysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                .Select(day => new DateTime(year, month, day))
                .ToList();

            var calendarDays = await _publicHolidayService.GetMonthCalendarAsync(year, month, "CZ");

            var employee = await _dbContext.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return new List<CalendaryDayDTO>();
            }

            var records = await _dbContext.AttenanceRecords
                .Where(r => r.EmployeeId == employeeId && r.Date.Year == year && r.Date.Month == month)
                .ToListAsync();

            var result = new List<CalendaryDayDTO>();

            foreach (var date in daysInMonth)
            {
                var holiday = calendarDays.FirstOrDefault(cd => cd.Date.Date == date.Date);
                var record = records.FirstOrDefault(r => r.Date.Date == date.Date);

                result.Add(new CalendaryDayDTO
                {
                    Date = date,
                    DayType = holiday?.Type.ToString() ?? DayType.Workday.ToString(),
                    HolidayName = holiday?.HolidayName,
                    EmployeeName = employee.LastName,
                    EmployeeId = employee.Id,

                    AttenanceIn = record?.AttenanceIn?.ToString(@"hh\:mm"),
                    AttenanceOut = record?.AttenanceOut?.ToString(@"hh\:mm"),
                    DoctorIn = record?.DoctorIn?.ToString(@"hh\:mm"),
                    DoctorOut = record?.DoctorOut?.ToString(@"hh\:mm"),
                    SmokeIn = record?.SmokeIn?.ToString(@"hh\:mm"),
                    SmokeOut = record?.SmokeOut?.ToString(@"hh\:mm"),
                    IsVacation = record?.IsVacation ?? false,
                    IsSickLeave = record?.IsSickLeave ?? false
                });
            }

            return result;
        }
    }
}
