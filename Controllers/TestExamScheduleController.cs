using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers;


[Route("api/[controller]")]
[ApiController]
public class TestExamScheduleController : ControllerBase
{
    private readonly ITestExamScheduleService _testExamScheduleService;


    public TestExamScheduleController(ITestExamScheduleService testExamScheduleService)
    {
        _testExamScheduleService = testExamScheduleService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllDisciplinesAsync(
        DateTimeOffset? mount, bool week, int? departmentId
      )
    {
        try
        {
            var result = await _testExamScheduleService.GetExamScheduleAsync(mount, week ,departmentId);
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
    
    
  
    [HttpGet ("detail")]
    public async Task<IActionResult> GetAllDisciplinDetal(
        DateTimeOffset startDate
    )
    {
        try
        {
            var result = await _testExamScheduleService.GetExamScheduleDetailAsync(startDate);
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
    
    [HttpGet ("detail/student-teacher")]
    public async Task<IActionResult> GetDetailForStudentAndTeacher(
        DateTimeOffset startDate
    )
    {
        try
        {
            var result = await _testExamScheduleService.GetExamScheduleDetailForStudentAndTeacherAsync(startDate);
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
    
    [HttpDelete("{id?}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var response = await _testExamScheduleService.DeleteExamScheduleDetailByIdAsync(id);
        if (response.Status == 1)
        {
            return BadRequest(new ApiResponse<Object>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<Object>(response.Status, response.Message, response.Data));
    }
    
    
}