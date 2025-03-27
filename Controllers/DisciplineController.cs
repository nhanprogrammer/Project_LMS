
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Services;
using Project_LMS.Services;

namespace Project_LMS.Controllers;

[Authorize(Policy = "STUDENT-REC-VIEW")]
[ApiController]
[Route("api/[controller]")]
public class DisciplineController : ControllerBase
{
    private readonly IDisciplinesService _disciplinesService;
    private readonly IStudentService _studentService;

    public DisciplineController(IDisciplinesService disciplinesService, IStudentService studentService)
    {
        _disciplinesService = disciplinesService;
        _studentService = studentService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<RewardResponse>>> GetById(int id)
    {
        var result = _disciplinesService.GetByIdAsync(id);
        return Ok(result);
    }

    [Authorize(Policy = "STUDENT-REC-INSERT")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<RewardResponse>>> Create(DisciplineRequest request)
    {
        try
        {
            var ressult = await _disciplinesService.AddAsync(request);
            return Ok(ressult);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse<string>(1, ex.Message));
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new ApiResponse<List<ValidationError>>(1, "Validation failed.", ex.Errors));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi tạo kỷ luật", ex.Message));
        }
    }

    [Authorize(Policy = "STUDENT-REC-UPDATE")]
    [HttpPut]
    public async Task<ActionResult<ApiResponse<RewardResponse>>> Update(UpdateDisciplineRequest request)
    {
        try
        {
            var ressult = await _disciplinesService.UpdateAsync(request);
            return Ok(ressult);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse<string>(1, ex.Message));
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new ApiResponse<List<ValidationError>>(1, "Validation failed.", ex.Errors));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi cập nhật kỷ luật", ex.Message));
        }
    }

    [Authorize(Policy = "STUDENT-REC-DELETE")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<RewardResponse>>> Delete(int id)
    {
        try
        {
            var result = await _disciplinesService.DeleteAsync(id);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse<string>(1, ex.Message, null));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi xóa kỷ luật", ex.Message));
        }
    }

    [HttpGet("getallstudentdiscipline")]
    public async Task<IActionResult> GetAllStudentOfReward([FromQuery] int academicId, [FromQuery] int departmentId, [FromQuery] PaginationRequest request, [FromQuery] string column, [FromQuery] bool orderBy)
    {
        var resutlt = await _studentService.GetAllStudentOfRewardOrDisciplines(false, academicId, departmentId, request, column, orderBy, null);
        return Ok(resutlt);
    }
    
    [HttpGet("searchstudentdiscipline")]
    public async Task<IActionResult> SearchStudentOfReward([FromQuery] int academicId, [FromQuery] int departmentId, [FromQuery] PaginationRequest request, [FromQuery] string column, [FromQuery] bool orderBy, [FromQuery] string searchItem)
    {
        var resutlt = await _studentService.GetAllStudentOfRewardOrDisciplines(false, academicId, departmentId, request, column, orderBy, searchItem);
        return Ok(resutlt);
    }
}
