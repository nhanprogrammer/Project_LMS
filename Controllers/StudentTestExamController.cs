using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers;

[Authorize(Policy = "STUDENT")]
[ApiController]
[Route("api/[controller]")]
public class StudentTestExamController : ControllerBase
{
    private readonly IStudentTestExamService _studentTestExamService;
    private readonly IAuthService _authService;
    public StudentTestExamController(IStudentTestExamService studentTestExamService, IAuthService authService)
    {
        _studentTestExamService = studentTestExamService;
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
        string? tab)
    {
        try
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));
            
            var result = await _studentTestExamService.GetStudentTestExamAsync(user.Id, pageNumber, pageSize, sortDirection, topicName, subjectId, departmentId, startDate,tab);
            if (result.Status == 1)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Status = 1, Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Status = 1, Message = $"Có lỗi xảy ra: {ex.Message}" });
        }
    }

    [HttpGet("{id?}")]
    public async Task<IActionResult> GetById(int id)
    {  var user = await _authService.GetUserAsync();
        if (user == null)
            return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));
        
        var response = await _studentTestExamService.GetStudentTestExamByIdAsync(id,user.Id);

        if (response.Status == 1)
        {
            return BadRequest(
                new ApiResponse<List<QuestionResponse>>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<List<QuestionResponse>>(response.Status, response.Message, response.Data));
    }
    
    
    [HttpPost("TN")]
    public async Task<IActionResult> SubmitAsignmentTN (SubmitMultipleChoiceQuestionRequest request)
    {  var user = await _authService.GetUserAsync();
        if (user == null)
            return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));
        
        var response = await _studentTestExamService.SubmitYourAssignment(user.Id, request);

        if (response.Status == 1)
        {
            return BadRequest(
                new ApiResponse<Object>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<Object>(response.Status, response.Message, response.Data));
    }
    [HttpPost("TL")]
    public async Task<IActionResult> SubmitAsignmentTL (SaveEssayRequest request)
    {  var user = await _authService.GetUserAsync();
        if (user == null)
            return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));
        
        var response = await _studentTestExamService.SaveEssay(user.Id, request);

        if (response.Status == 1)
        {
            return BadRequest(
                new ApiResponse<Object>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<Object>(response.Status, response.Message, response.Data));
    }


}