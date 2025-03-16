using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
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

        public async Task<SystemSettingResponse> GetById(int id)
        {
            var setting = await _context.SystemSettings.FindAsync(id);
            if (setting == null) throw new KeyNotFoundException("Không tìm thấy cài đặt hệ thống.");

            return new SystemSettingResponse
            {
                Id = setting.Id,
                CaptchaEnabled = setting.CaptchaEnabled,
                CurrentTheme = setting.CurrentTheme,
                Language = setting.Language,
                CreateAt = setting.CreateAt
            };
        }

        public async Task<IEnumerable<SystemSettingResponse>> GetAll()
        {
            return await _context.SystemSettings
                .Select(s => new SystemSettingResponse
                {
                    Id = s.Id,
                    CaptchaEnabled = s.CaptchaEnabled,
                    CurrentTheme = s.CurrentTheme,
                    Language = s.Language,
                    CreateAt = s.CreateAt
                })
                .ToListAsync();
        }

        public async Task<SystemSettingResponse> Create(SystemSettingRequest request)
        {
            var setting = new SystemSetting
            {
                CaptchaEnabled = request.CaptchaEnabled,
                CurrentTheme = request.CurrentTheme,
                Language = request.Language,
                CreateAt = DateTime.UtcNow
            };

            _context.SystemSettings.Add(setting);
            await _context.SaveChangesAsync();

            return new SystemSettingResponse
            {
                Id = setting.Id,
                CaptchaEnabled = setting.CaptchaEnabled,
                CurrentTheme = setting.CurrentTheme,
                Language = setting.Language,
                CreateAt = setting.CreateAt
            };
        }

        public async Task<SystemSettingResponse> Update(int id, SystemSettingRequest request)
        {
            var setting = await _context.SystemSettings.FindAsync(id);
            if (setting == null) throw new KeyNotFoundException("Không tìm thấy cài đặt hệ thống.");

            setting.CaptchaEnabled = request.CaptchaEnabled;
            setting.CurrentTheme = request.CurrentTheme;
            setting.Language = request.Language;
            setting.UpdateAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new SystemSettingResponse
            {
                Id = setting.Id,
                CaptchaEnabled = setting.CaptchaEnabled,
                CurrentTheme = setting.CurrentTheme,
                Language = setting.Language,
                CreateAt = setting.CreateAt
            };
        }
        public async Task<bool> Delete(int id)
        {
            var systemSetting = await _context.SystemSettings.FindAsync(id);
            if (systemSetting == null || systemSetting.IsDelete == true)
                throw new KeyNotFoundException("Không tìm thấy cài đặt hệ thống");

            systemSetting.IsDelete = true;
            systemSetting.UpdateAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
