using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemSettingController : ControllerBase
    {
        private readonly ISystemSettingService _systemSettingService;
        
        public SystemSettingController(ISystemSettingService systemSettingService)
        {
            _systemSettingService = systemSettingService;
        }

        //[HttpGet("user/{id}")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    try
        //    {
        //        var result = await _systemSettingService.GetById(id);
        //        return Ok(new ApiResponse<object>(0, "Tìm thấy", result));
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new ApiResponse<object>(1, ex.Message));
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _systemSettingService.GetAll();
            return Ok(new ApiResponse<object>(0, "Tìm thấy", result));
        }

        //[HttpPut("user/{userId}")]
        //public async Task<IActionResult> Update(int userId, [FromBody] SystemSettingRequest request)
        //{
        //    var result = await _systemSettingService.UpdateByUserId(userId, request);
        //    return Ok(new ApiResponse<SystemSettingResponse>(0, "Cập nhật thành công!", result));
        //}
        [Authorize]
        [HttpPut("update-setting")]
        public async Task<IActionResult> UpdateSetting([FromBody] SystemSettingRequest request)
        {
            var response = await _systemSettingService.UpdateByUserId(request);
            return Ok(new ApiResponse<UserSystemSettingResponse>(0, "Cập nhật thành công!", response));
        }

        [Authorize(Policy = "DATA-MNG-VIEW")]
        [HttpGet("user-setting")]
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
