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
        public async Task<List<CalendaryDayDTO>> GetCalendarWithAttendance(int year, int month, int? departmentId = null, int? employeeId = null)
        {
            var daysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                .Select(day => new DateTime(year, month, day))
                .ToList();

            var calendarDays = await _publicHolidayService.GetMonthCalendarAsync(year, month, "CZ");
            var employeesQuery = _dbContext.Employees.AsQueryable();

            if (departmentId != null)
            {
                employeesQuery = employeesQuery.Where(e => e.DepartmentId == departmentId);
            }

            var employees = await employeesQuery.ToListAsync();
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

                    TimeSpan? worked = null;
                    if (record?.AttenanceIn != null && record?.AttenanceOut != null)
                    {
                        worked = record?.AttenanceOut - record?.AttenanceIn;
                    }

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
                        IsSickLeave = record?.IsSickLeave ?? false,
                        WorkedHours = worked.HasValue ? $"{(int)worked.Value.TotalHours:D2}:{worked.Value.Minutes:D2}" : "0",


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

                TimeSpan? worked = null;
                if (record?.AttenanceIn != null && record?.AttenanceOut != null)
                {
                    worked = record?.AttenanceOut - record?.AttenanceIn;
                }

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
                    IsSickLeave = record?.IsSickLeave ?? false,
                    WorkedHours = worked.HasValue ? $"{(int)worked.Value.TotalHours:D2}:{worked.Value.Minutes:D2}" : "0"
                });
            }

            return result;
        }
        //Editace dochazky - kazdy kalendarni den zvlast
        public async Task<AttenanceRecordDTO> GetAttendanceByEmployeeAndDateAsync(int employeeId, DateTime date)
        {
            var record = await _dbContext.AttenanceRecords
                .FirstOrDefaultAsync(r => r.EmployeeId == employeeId && r.Date.Date == date.Date);

            if (record == null)
            {
                return null;
            }

            return new AttenanceRecordDTO
            {
                EmployeeId = record.EmployeeId,
                Date = record.Date,
                AttenanceIn = record.AttenanceIn,
                AttenanceOut = record.AttenanceOut,
                DoctorIn = record.DoctorIn,
                DoctorOut = record.DoctorOut,
                IsVacation = record.IsVacation,
                IsSickLeave = record.IsSickLeave,
                WorkedHours = record.WorkedHours
            };
        }
        public async Task UpdateAttendanceAsync(AttenanceRecordDTO dto)
        {
            var record = await _dbContext.AttenanceRecords
                .FirstOrDefaultAsync(r => r.EmployeeId == dto.EmployeeId && r.Date.Date == dto.Date.Date);

            if (record == null)
            {
                
                throw new Exception("Attendance record not found.");
            }

            record.AttenanceIn = dto.AttenanceIn;
            record.AttenanceOut = dto.AttenanceOut;
            record.DoctorIn = dto.DoctorIn;
            record.DoctorOut = dto.DoctorOut;
            record.IsVacation = dto.IsVacation;
            record.IsSickLeave = dto.IsSickLeave;

            await _dbContext.SaveChangesAsync();
        }
    }
}
