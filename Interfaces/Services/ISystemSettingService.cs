﻿using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ISystemSettingService
{
    Task<SystemSettingResponse> GetById(int id);
    Task<IEnumerable<SystemSettingResponse>> GetAll();
    Task<SystemSettingResponse> Create(SystemSettingRequest request);
    Task<SystemSettingResponse> UpdateByUserId(int userId, SystemSettingRequest request);
    Task<bool> Delete(int id);
}