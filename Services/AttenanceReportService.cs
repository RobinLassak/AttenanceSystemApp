using AttenanceSystemApp.DTO;

namespace AttenanceSystemApp.Services
{
    public class AttenanceReportService
    {
        public List<EmployeeMonthlySummaryDTO> GetMonthlySummaries(List<CalendaryDayDTO> calendarDays)
        {
            return calendarDays
                .GroupBy(x => x.EmployeeName)
                .Select(g => new EmployeeMonthlySummaryDTO
                {
                    EmployeeName = g.Key,
                    TotalWorkedHours = (int)Math.Round(g.Sum(x =>
                    {
                        if (TimeSpan.TryParse(x.WorkedHours, out var span))
                            return span.TotalHours;
                        return 0;
                    })),
                    TotalDoctorHours = (int)Math.Round(g.Sum(x =>
                    {
                        if (TimeSpan.TryParse(x.DoctorIn, out var start) && TimeSpan.TryParse(x.DoctorOut, out var end))
                            return (end - start).TotalHours;
                        return 0;
                    })),
                    VacationDays = g.Count(x => x.IsVacation),
                    SickLeaveDays = g.Count(x => x.IsSickLeave),
                    DaysPresent = g.Count(x => x.WorkedTimeSpan.HasValue && x.WorkedTimeSpan.Value.TotalMinutes > 0)
                })
                .ToList();
        }
    }
}
