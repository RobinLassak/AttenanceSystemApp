using AttenanceSystemApp.DTO;
using AttenanceSystemApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AttenanceSystemApp.Services
{
    public class AttenanceRecordService
    {
        private readonly AttenanceDbContext _dbContext;
        public AttenanceRecordService(AttenanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //Vytvoreni noveho zaznamu dochazky
        internal async Task RecordAttenance(AttenanceRecordDTO newRecord)
        {
            var today = DateTime.Today;
            var record = await _dbContext.AttenanceRecords.FirstOrDefaultAsync(r => r.EmployeeId == newRecord.EmployeeId
            && r.Date == today);

            if (record == null)
            {
                record = new AttenanceRecord
                {
                    EmployeeId = newRecord.EmployeeId,
                    Date = today,
                };
                _dbContext.AttenanceRecords.Add(record);
            }

            var now = DateTime.Now.TimeOfDay;
            switch (newRecord.AttenanceType)
            {
                case "AttenanceIn":
                    record.AttenanceIn = now;
                    break;
                case "AttenanceOut":
                    if(record.AttenanceIn == null)
                    {
                        throw new InvalidOperationException("You need to record arrival at first!");
                    }
                    record.AttenanceOut = now;
                    break;
                case "DoctorIn":
                    record.DoctorIn = now;
                    break;
                case "DoctorOut":
                    if (record.DoctorIn == null)
                    {
                        throw new InvalidOperationException("You need to record arrival at first!");
                    }
                    record.DoctorOut = now;
                    break;
                case "SmokeIn":
                    record.SmokeIn = now;
                    break;
                case "SmokeOut":
                    if (record.SmokeIn == null)
                    {
                        throw new InvalidOperationException("You need to record arrival at first!");
                    }
                    record.SmokeOut = now;
                    break;
                case "IsVacationOn":
                    record.IsVacation = true;
                    break;
                case "IsVacationOff":
                    if (record.IsVacation == false)
                    {
                        throw new InvalidOperationException("You need to record arrival at first!");
                    }
                    record.IsVacation = false;
                    break;
                case "IsSickLeaveOn":
                    record.IsSickLeave = true;
                    break;
                case "IsSickLeaveOff":
                    if (record.IsSickLeave == false)
                    {
                        throw new InvalidOperationException("You need to record arrival at first!");
                    }
                    record.IsSickLeave = false;
                    break;
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
