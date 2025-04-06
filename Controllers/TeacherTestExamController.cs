using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers;

[Authorize(Policy = "TEACHER")]
[Route("api/[controller]")]
[ApiController]
public class TeacherTestExamController : ControllerBase
{
    private readonly ITeacherTestExamService _teacherTestExamService;
    private readonly IAuthService _authService;

    public TeacherTestExamController(ITeacherTestExamService teacherTestExamService , IAuthService authService)
    {
        _teacherTestExamService = teacherTestExamService;
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDisciplinesAsync(   int? pageNumber,
        int? pageSize,
        string? sortDirection,
        string? topicName,
        int? subjectId,
        int? departmentId,
        string? startDate,
        string? tab
        
        )
    {
        var user = await _authService.GetUserAsync();
        if (user == null)
            return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));
        var response = await _teacherTestExamService.GetTeacherTestExamAsync(user.Id,
            pageNumber, pageSize, sortDirection, topicName, subjectId, departmentId, startDate , tab);

        if (response.Status == 1)
        {
            return BadRequest(
                new ApiResponse<PaginatedResponse<TeacherTestExamResponse>>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<PaginatedResponse<TeacherTestExamResponse>>(response.Status, response.Message, response.Data));
    }

    [HttpGet("detail")]
    public async Task<IActionResult> UpdateDiscipline(int id, int classId)
    {
        var response = await _teacherTestExamService.GetTeacherTestExamById(id, classId);

        if (response.Status == 1)
        {
            return BadRequest(new ApiResponse<object>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<object>(response.Status, response.Message, response.Data));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Object>>> Create([FromBody] TeacherTestExamRequest request)
    {
        var user = await _authService.GetUserAsync();
        if (user == null)
            return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

        var response = await _teacherTestExamService.CreateTeacherTestExamAsync(user.Id,request);

        if (response.Status == 1)
        {
            return BadRequest(
                new ApiResponse<Object>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<Object>(response.Status, response.Message, response.Data));
    }
    
    [HttpPut]
    public async Task<ActionResult<ApiResponse<Object>>> Update([FromBody] TeacherTestExamRequest request)
    {
        var user = await _authService.GetUserAsync();
        if (user == null)
            return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

        
        var response = await _teacherTestExamService.UpdateTeacherTestExamAsync(user.Id,request);

        if (response.Status == 1)
        {
            return BadRequest(
                new ApiResponse<Object>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<Object>(response.Status, response.Message, response.Data));
    }
    
    
    [HttpPut("start-exam")]
    public async Task<ActionResult<ApiResponse<Object>>> StartExam([FromBody] StartTestExamRequest request)
    {
        var response = await _teacherTestExamService.StarTeacherTestExamById(request);

        if (response.Status == 1)
        {
            return BadRequest(
                new ApiResponse<Object>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<Object>(response.Status, response.Message, response.Data));
    }
    
    [HttpGet("Filter")]
    public async Task<ActionResult<ApiResponse<Object>>> FilterClass([FromQuery] int departmentId )
    {
        var response = await _teacherTestExamService.GetFilterClass(departmentId);

        if (response.Status == 1)
        {
            return BadRequest(
                new ApiResponse<Object>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<Object>(response.Status, response.Message, response.Data));
    }
    
    
    [HttpDelete("{id?}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var response = await _teacherTestExamService.DeleteTeacherTestExamById(id);
        if (response.Status == 1)
        {
            return BadRequest(new ApiResponse<Object>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<Object>(response.Status, response.Message, response.Data));
    }
}