using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [Authorize(Policy = "DATA-MNG-VIEW")]
    [Route("api/[controller]")]
    [ApiController]
    public class SystemSettingController : ControllerBase
    {
        private readonly ISystemSettingService _systemSettingService;
        
        public SystemSettingController(ISystemSettingService systemSettingService)
        {
            _systemSettingService = systemSettingService;
        }

        // [HttpGet]
        // public async Task<IActionResult> GetAll()
        // {
        //     var result = await _systemSettingService.GetAll();
        //     return Ok(new ApiResponse<object>(0, "Tìm thấy", result));
        // }

        [Authorize(Policy = "DATA-MNG-INSERT")]
        [HttpPut]
        public async Task<IActionResult> UpdateSetting([FromBody] SystemSettingRequest request)
        {
            var response = await _systemSettingService.UpdateByUserId(request);
            return Ok(new ApiResponse<UserSystemSettingResponse>(0, "Cập nhật thành công!", response));
        }

        [Authorize(Policy = "DATA-MNG-VIEW")]
        [HttpGet]
        public async Task<IActionResult> GetUserSetting()
        {
            try
            {
                var response = await _systemSettingService.GetCurrentUserSettingAsync();
                return Ok(new ApiResponse<UserSystemSettingResponse>(0, "Lấy cài đặt thành công!", response));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(1, "Lỗi hệ thống: " + ex.Message, null));
            }
        }

    }
}
