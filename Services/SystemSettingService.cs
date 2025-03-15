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
        try
        {
            if (string.IsNullOrEmpty(request.Language)) throw new ArgumentException("Ngôn ngữ không được để trống.");

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
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi Create: {ex.Message} | {ex.StackTrace}");
            throw;
        }
    }


    public async Task<SystemSettingResponse> Update(int id, SystemSettingRequest request)
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi Update: {ex.Message} | {ex.StackTrace}");
            throw;
        }
    }


}