using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ISystemSettingService
{
    Task<UserSystemSettingResponse> GetCurrentUserSettingAsync();
    Task<SystemSettingResponse> GetById(int id);
    Task<IEnumerable<SystemSettingResponse>> GetAll();
    Task<UserSystemSettingResponse> UpdateByUserId(SystemSettingRequest request);
}