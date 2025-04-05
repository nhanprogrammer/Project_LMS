using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Project_LMS.Controllers;


[Authorize(Policy = "TEACHER")]
[Route("api/[controller]")]
[ApiController]
public class GradeEntryController : ControllerBase
{
    private readonly IGradeEntryService _gradeEntryService;
    private readonly IAuthService _authService;
    private readonly ILogger<GradeEntryController> _logger;

    public GradeEntryController(IGradeEntryService gradeEntryService, IAuthService authService, ILogger<GradeEntryController> logger)
    {
        _gradeEntryService = gradeEntryService;
        _authService = authService;
        _logger = logger;
    }
    [Authorize(Policy = "TEACHER")]
    [HttpGet("test/{testId}")]
    public async Task<IActionResult> GetGradingData(int testId, [FromQuery] int? classId = null)
    {
        var user = await _authService.GetUserAsync();
        int teacherId = user.Id;
        var response = await _gradeEntryService.GetGradingData(testId, teacherId, classId);

        if (response.Status == 0)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpPut("save")]
    public async Task<IActionResult> SaveGrades([FromBody] SaveGradesRequest request)
    {
        // Lấy teacherId từ token JWT
        var teacherIdClaim = await _authService.GetUserAsync();
        
        _logger.LogInformation($"SaveGrades - TestId: {request.TestId}, ClassId: {request.ClassId}, TeacherId: {teacherIdClaim.Id}, Grades count: {request.Grades?.Count ?? 0}");

        // Gọi service để lưu điểm
        var response = await _gradeEntryService.SaveGrades(request, teacherIdClaim.Id);

        if (response.Status == 0)
        {
            return Ok(new ApiResponse<object>(0, "Đã chốt điểm thành công!"));
        }

        return BadRequest(response);
    }
}