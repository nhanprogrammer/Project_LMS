using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;

namespace Project_LMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonController : ControllerBase
{
    private readonly ILessonService _lessonService;
    private readonly IAuthService _authService;

    public LessonController(ILessonService lessonService, IAuthService authService)
    {
        _lessonService = lessonService;
        _authService = authService;
    }

    [Authorize(Policy = "TEACHER-REC-VIEW")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<LessonResponse>>>> GetAll(
        [FromQuery] string? keyword = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

            var response = await _lessonService.GetLessonAsync(keyword, pageNumber, pageSize);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>(1, $"Lỗi server: {ex.Message}", null));
        }
    }

    [Authorize(Policy = "TEACHER-REC-VIEW")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> GetById(int id)
    {
        try
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

            var result = await _lessonService.GetLessonByIdAsync(id);
            if (result.Data == null)
            {
                return NotFound(new ApiResponse<LessonResponse>(1, "Không tìm thấy bài học", null));
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>(1, $"Lỗi server: {ex.Message}", null));
        }
    }

    [Authorize(Policy = "TEACHER-REC-INSERT")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> Create([FromBody] CreateLessonRequest request)
    {
        try
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<LessonResponse>(1, "Dữ liệu không hợp lệ", null));
            }

            var result = await _lessonService.CreateLessonAsync(request);
            if (result.Status != 0)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>(1, $"Lỗi server: {ex.Message}", null));
        }
    }

    [Authorize(Policy = "TEACHER-REC-UPDATE")]
    [HttpPut]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> Update([FromBody] CreateLessonRequest request)
    {
        try
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(new ApiResponse<LessonResponse>(1, "Dữ liệu không hợp lệ", null));
            }

            var result = await _lessonService.UpdateLessonAsync(request);
            if (result.Status != 0)
            {
                if (result.Message.Contains("not found"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>(1, $"Lỗi server: {ex.Message}", null));
        }
    }

    [Authorize(Policy = "TEACHER-REC-DELETE")]
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteMultiple([FromBody] DeleteMultipleRequest request)
    {
        try
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

            if (request?.Ids == null || !request.Ids.Any())
            {
                return BadRequest(new ApiResponse<bool>(1, "Chưa chọn bài học để xóa", false));
            }

            var result = await _lessonService.DeleteMultipleLessonsAsync(request.Ids);
            if (result.Status == 0)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<bool>(1, $"Lỗi khi xóa bài học: {ex.Message}", false));
        }
    }
}