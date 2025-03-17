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

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _systemSettingService.GetById(id);
                return Ok(new ApiResponse<object>(0, "Success", result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>(1, ex.Message));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _systemSettingService.GetAll();
            return Ok(new ApiResponse<object>(0, "Success", result));
        }

        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] SystemSettingRequest request)
        //{
        //    try
        //    {
        //        var result = await _systemSettingService.Create(request);
        //        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new ApiResponse<object>(0, "Thêm mới thành công", result));
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(new ApiResponse<object>(1, ex.Message));
        //    }
        //}

        [HttpPut("user/{userId}")]
        public async Task<IActionResult> Update(int userId, [FromBody] SystemSettingRequest request)
        {
            var result = await _systemSettingService.UpdateByUserId(userId, request);
            return Ok(new ApiResponse<SystemSettingResponse>(0, "Cập nhật thành công!", result));
        }


    }
}
