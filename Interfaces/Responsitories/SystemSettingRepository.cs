using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories
{
    public class SystemSettingRepository: ISystemSettingService
    {
        private readonly ApplicationDbContext _context;

        public SystemSettingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SystemSetting>> GetAll()
        {
            return await _context.SystemSettings
                .Where(s => s.IsDelete == false)
                .ToListAsync();
        }

        public async Task<SystemSetting?> GetById(int id)
        {
            return await _context.SystemSettings.FindAsync(id);
        }

        public async Task<SystemSetting> Add(SystemSetting systemSetting)
        {
            _context.SystemSettings.Add(systemSetting);
            await _context.SaveChangesAsync();
            return systemSetting;
        }

        public async Task<SystemSetting?> Update(int id, SystemSetting systemSetting)
        {
            var existing = await _context.SystemSettings.FindAsync(id);
            if (existing == null) return null;

            existing.CaptchaEnabled = systemSetting.CaptchaEnabled;
            existing.CurrentTheme = systemSetting.CurrentTheme;
            existing.UpdateAt = DateTime.UtcNow;
            existing.UserUpdate = systemSetting.UserUpdate;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> Delete(int id)
        {
            var systemSetting = await _context.SystemSettings.FindAsync(id);
            if (systemSetting == null) return false;

            systemSetting.IsDelete = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
