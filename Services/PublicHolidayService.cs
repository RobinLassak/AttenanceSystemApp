using AttenanceSystemApp.DTO;
using AttenanceSystemApp.Models;
using System.Text.Json;

namespace AttenanceSystemApp.Services
{
    public class PublicHolidayService
    {
        private readonly HttpClient _httpClient;

        public PublicHolidayService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CalendaryDay>> GetMonthCalendarAsync(int year, int month, string countryCode)
        {
            var publicHolidays = await GetPublicHolidaysAsync(year, countryCode);

            var daysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                .Select(day => new DateTime(year, month, day))
                .ToList();

            var calendarDays = daysInMonth.Select(date =>
            {
                var holiday = publicHolidays.FirstOrDefault(h => h.Date == date);

                return new CalendaryDay
                {
                    Date = date,
                    Type = holiday != null
                        ? DayType.PublicHoliday
                        : (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                            ? DayType.Weekend
                            : DayType.Workday,
                    HolidayName = holiday?.LocalName
                };
            }).ToList();
            return calendarDays;
        }

        private async Task<List<PublicHolidayDTO>> GetPublicHolidaysAsync(int year, string countryCode)
        {
            var url = $"https://date.nager.at/api/v3/PublicHolidays/{year}/{countryCode}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<PublicHolidayDTO>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<PublicHolidayDTO>();
        }
    }
}
