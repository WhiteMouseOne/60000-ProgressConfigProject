using Microsoft.EntityFrameworkCore;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.Order;
using Progress.Model.Entitys;
using Progress.Repository;

namespace Progress.Service.SystemManagement
{
    public class AlertSettingService : IAlertSettingService
    {
        private readonly ProgressDbContext _db;

        public AlertSettingService(ProgressDbContext db)
        {
            _db = db;
        }

        public async Task<AlertSettingDto?> GetAsync()
        {
            var s = await _db.AlertSettings!.AsNoTracking().FirstOrDefaultAsync();
            if (s == null) return null;
            return new AlertSettingDto { Id = s.Id, LeadDays = s.LeadDays, Enabled = s.Enabled };
        }

        public async Task SaveAsync(AlertSettingDto dto)
        {
            var s = await _db.AlertSettings!.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (s == null)
            {
                s = new AlertSetting { LeadDays = dto.LeadDays, Enabled = dto.Enabled };
                _db.AlertSettings.Add(s);
            }
            else
            {
                s.LeadDays = dto.LeadDays;
                s.Enabled = dto.Enabled;
            }
            await _db.SaveChangesAsync();
        }
    }
}
