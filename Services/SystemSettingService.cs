using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services;

public class SystemSettingService : ISystemSettingService
{
    private readonly ApplicationDbContext _context;

    public SystemSettingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SystemSettingResponse> GetById(int userId)
    {
        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(x => x.UserId == userId && x.IsDelete != true);

        if (setting == null) throw new KeyNotFoundException("Không tìm thấy cài đặt hệ thống cho người dùng này.");

        return new SystemSettingResponse
        {
            Id = setting.Id,
            CaptchaEnabled = setting.CaptchaEnabled,
            CurrentTheme = setting.CurrentTheme,
            Language = setting.Language,
            CreateAt = setting.CreateAt,
            UpdateAt = setting.UpdateAt
        };
    }


    public async Task<IEnumerable<SystemSettingResponse>> GetAll()
    {
        var settings = await _context.SystemSettings
                        .Where(x => x.IsDelete == false)
                        .ToListAsync();

        return settings.Select(s => new SystemSettingResponse
        {
            Id = s.Id,
            CaptchaEnabled = s.CaptchaEnabled,
            CurrentTheme = s.CurrentTheme,
            Language = s.Language,
            CreateAt = s.CreateAt,
            UpdateAt = s.UpdateAt
        });
    }

    public async Task<SystemSettingResponse> Create(SystemSettingRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Language)) throw new ArgumentException("Ngôn ngữ không được để trống.");

            var setting = new SystemSetting
            {
                CaptchaEnabled = request.CaptchaEnabled,
                CurrentTheme = request.CurrentTheme,
                Language = request.Language,
                CreateAt = DateTime.Now
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
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi tạo: {ex.Message} | {ex.StackTrace}");
            throw;
        }
    }


    public async Task<SystemSettingResponse> UpdateByUserId(int userId, SystemSettingRequest request)
    {
        try
        {
            var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.UserId == userId && s.IsDelete != true);
            if (setting == null) throw new KeyNotFoundException("Không tìm thấy cài đặt hệ thống theo UserId.");

            setting.CaptchaEnabled = request.CaptchaEnabled;
            setting.CurrentTheme = request.CurrentTheme;
            setting.Language = request.Language;
            setting.UpdateAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new SystemSettingResponse
            {
                Id = setting.Id,
                CaptchaEnabled = setting.CaptchaEnabled,
                CurrentTheme = setting.CurrentTheme,
                Language = setting.Language,
                CreateAt = setting.CreateAt,
                UpdateAt = setting.UpdateAt
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi cập nhật: {ex.Message} | {ex.StackTrace}");
            throw;
        }
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