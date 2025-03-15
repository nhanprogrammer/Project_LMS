using Microsoft.AspNetCore.Mvc;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubjectGroupController : ControllerBase
{
    private readonly ISubjectGroupService _subjectGroupService;

    public SubjectGroupController(ISubjectGroupService subjectGroupService)
    {
        _subjectGroupService = subjectGroupService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAllDisciplinesAsync(    int? pageNumber,
        int? pageSize,
        string? sortDirection)
    {
        var response = await _subjectGroupService.GetAllSubjectGroupAsync(pageNumber, pageSize, sortDirection);

        if (response.Status == 1)
        {
            return BadRequest(
                new ApiResponse<PaginatedResponse<SubjectGroupResponse>>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<PaginatedResponse<SubjectGroupResponse>>(response.Status, response.Message, response.Data));
    }

    [HttpPost]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateSubjectGroupRequest createSubjectGroupRequest)
    {
        var response = await _subjectGroupService.CreateSubjectGroupAsync(createSubjectGroupRequest);

        if (response.Status == 1)
        {
            return BadRequest(
                new ApiResponse<SubjectGroupResponse>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<SubjectGroupResponse>(response.Status, response.Message, response.Data));
    }

    [HttpGet("{id?}")]
    public async Task<IActionResult> UpdateDiscipline(int id)
    {
        var response = await _subjectGroupService.GetSubjectGroupById(id);

        if (response.Status == 1)
        {
            return BadRequest(new ApiResponse<SubjectGroupResponse>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<SubjectGroupResponse>(response.Status, response.Message, response.Data));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateDepartment([FromBody] UpdateSubjectGroupRequest updateSubjectGroupRequest)
    {
        var response = await _subjectGroupService.UpdateSubjectGroupAsync(updateSubjectGroupRequest);
        if (response.Status == 1)
        {
            return BadRequest(new ApiResponse<SubjectGroupResponse>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<SubjectGroupResponse>(response.Status, response.Message, response.Data));
    }

    [HttpDelete("{id?}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var response = await _subjectGroupService.DeleteSubjectGroupAsync(id);
        if (response.Status == 1)
        {
            return BadRequest(new ApiResponse<SubjectGroupResponse>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<SubjectGroupResponse>(response.Status, response.Message, response.Data));
    }

 [HttpDelete]
 public async Task<IActionResult> DeleteListSubject([FromBody] DeleteRequest deleteRequest)
{
    Console.WriteLine($"Received IDs: {string.Join(", ", deleteRequest)}");


    var response = await _subjectGroupService.DeleteSubjectGroupSubject(deleteRequest);
    
    if (response.Status == 1)
    {
        return BadRequest(new ApiResponse<SubjectGroupResponse>(response.Status, response.Message, response.Data));
    }

    return Ok(new ApiResponse<SubjectGroupResponse>(response.Status, response.Message, response.Data));
}

}