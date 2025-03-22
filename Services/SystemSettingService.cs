using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services;

public class SystemSettingService : ISystemSettingService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;

    public SystemSettingService(ApplicationDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<UserSystemSettingResponse> GetCurrentUserSettingAsync()
    {
        var user = await _authService.GetUserAsync();
        if (user == null)
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc đã hết hạn!");

        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.IsDelete != true);

        if (setting == null)
            throw new KeyNotFoundException("Không tìm thấy cài đặt hệ thống cho người dùng này.");

        return new UserSystemSettingResponse
        {
            UserId = setting.Id,
            CaptchaEnabled = setting.CaptchaEnabled,
            CurrentTheme = setting.CurrentTheme,
            Language = setting.Language
        };
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
            //CreateAt = setting.CreateAt,
            //UpdateAt = setting.UpdateAt
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
            //CreateAt = s.CreateAt,
            //UpdateAt = s.UpdateAt
        });
    }



    //public async Task<SystemSettingResponse> UpdateByUserId(int userId, SystemSettingRequest request)
    //{
    //    try
    //    {
    //        var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.UserId == userId && s.IsDelete != true);
    //        if (setting == null) throw new KeyNotFoundException("Không tìm thấy cài đặt hệ thống theo UserId.");

    //        setting.CaptchaEnabled = request.CaptchaEnabled;
    //        setting.CurrentTheme = request.CurrentTheme;
    //        setting.Language = request.Language;
    //        setting.UpdateAt = DateTime.Now;

    //        await _context.SaveChangesAsync();

    //        return new SystemSettingResponse
    //        {
    //            Id = setting.Id,
    //            CaptchaEnabled = setting.CaptchaEnabled,
    //            CurrentTheme = setting.CurrentTheme,
    //            Language = setting.Language,
    //            //CreateAt = setting.CreateAt,
    //            //UpdateAt = setting.UpdateAt
    //        };
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Lỗi khi cập nhật: {ex.Message} | {ex.StackTrace}");
    //        throw;
    //    }
    //}
    public async Task<UserSystemSettingResponse> UpdateByUserId(SystemSettingRequest request)
    {
        try
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                throw new UnauthorizedAccessException("Token không hợp lệ hoặc đã hết hạn!");

            var setting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.UserId == user.Id && s.IsDelete != true);

            if (setting == null)
                throw new KeyNotFoundException("Không tìm thấy cài đặt hệ thống cho người dùng.");

            // Cập nhật dữ liệu
            setting.CaptchaEnabled = request.CaptchaEnabled;
            setting.CurrentTheme = request.CurrentTheme;
            setting.Language = request.Language;
            setting.UpdateAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new UserSystemSettingResponse
            {
                UserId = setting.Id,
                CaptchaEnabled = setting.CaptchaEnabled,
                CurrentTheme = setting.CurrentTheme,
                Language = setting.Language,
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi cập nhật: {ex.Message} | {ex.StackTrace}");
            throw;
        }
    }

}