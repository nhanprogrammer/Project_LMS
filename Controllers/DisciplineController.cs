
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers;
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

    [HttpGet]
    public async Task<IActionResult> GetAllDisciplinesAsync()
    {
        var response = await _disciplinesService.GetAllDisciplineAsync();

        if (response.Status == 1)
        {
            return BadRequest(new ApiResponse<List<DisciplineResponse>>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<List<DisciplineResponse>>(response.Status, response.Message, response.Data));
    }

    [HttpPost]
    public async Task<IActionResult> CreateDisciplinesAsync([FromBody] CreateDisciplineRequest request)
    {
        var response = await _disciplinesService.CreateDisciplineAsync(request);

        if (response.Status == 1)
        {
            return BadRequest(new ApiResponse<DisciplineResponse>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<DisciplineResponse>(response.Status, response.Message, response.Data));
    }

    [HttpPut("{id?}")]
    public async Task<IActionResult> UpdateDiscipline(String id, [FromBody] UpdateDisciplineRequest request)
    {
        var response = await _disciplinesService.UpdateDisciplineAsync(id, request);
        if (response.Status == 1)
        {
            return BadRequest(new ApiResponse<DisciplineResponse>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<DisciplineResponse>(response.Status, response.Message, response.Data));
    }

    [HttpDelete("{id?}")]
    public async Task<IActionResult> DeleteDepartment(String id)
    {
        var response = await _disciplinesService.DeleteDisciplineAsync(id);
        if (response.Status == 1)
        {
            return BadRequest(
                new ApiResponse<DisciplineResponse>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<DisciplineResponse>(response.Status, response.Message, response.Data));
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
