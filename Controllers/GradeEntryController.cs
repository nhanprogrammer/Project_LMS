using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GradeEntryController : ControllerBase
{
    private readonly IGradeEntryService _gradeEntryService;
    private readonly IAuthService _authService;

    public GradeEntryController(IGradeEntryService gradeEntryService, IAuthService authService)
    {
        _gradeEntryService = gradeEntryService;
        _authService = authService;
    }

    [HttpGet("test/{testId}")]
    public async Task<IActionResult> GetGradingData(int testId)
    {
        var user = await _authService.GetUserAsync();
        int teacherId = user.Id; // Thay bằng cách lấy từ token thực tế
        var response = await _gradeEntryService.GetGradingData(testId, teacherId);

        if (response.Status == 0)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveGrades([FromBody] SaveGradesRequest request)
    {
        // Lấy teacherId từ token JWT
        var teacherIdClaim = await _authService.GetUserAsync();

        // Gọi service để lưu điểm
        var response = await _gradeEntryService.SaveGrades(request, teacherIdClaim.Id);

        if (response.Status == 0)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}