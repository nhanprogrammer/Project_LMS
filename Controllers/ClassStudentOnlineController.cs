using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;

namespace Project_LMS.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ClassStudentOnlineController : ControllerBase
{
private readonly IClassStudentsOnlineService _classStudentsOnlineService;

public ClassStudentOnlineController(IClassStudentsOnlineService classStudentsOnlineService)
{
    _classStudentsOnlineService = classStudentsOnlineService;
}

[HttpGet]
public async Task<IActionResult>  GetAllDepartment()
{
    var response = await _classStudentsOnlineService.GetAllClassStudentOnlineAsync();

    if (response.Status == 1)
    {
        return BadRequest(new ApiResponse<List<ClassStudentOnlineResponse>>(response.Status, response.Message,response.Data)); 
    }

    return Ok(new ApiResponse<List<ClassStudentOnlineResponse>>(response.Status, response.Message, response.Data));
}

[HttpPost]
public async Task<IActionResult> CreateDepartment([FromBody] CreateClassStudentOnlineRequest request)
{
    var response = await _classStudentsOnlineService.CreateClassStudentOnlineAsync(request);

    if (response.Status == 1)
    {
        return BadRequest(
            new ApiResponse<ClassStudentOnlineResponse>(response.Status, response.Message, response.Data));
    }

    return Ok(new ApiResponse<ClassStudentOnlineResponse>(response.Status, response.Message, response.Data));
}

[HttpPut("{id?}")]
public async Task<IActionResult> UpdateDepartment(String id, [FromBody] UpdateClassStudentOnlineRequest request)
{
    var response =   await _classStudentsOnlineService.UpdateClassStudentOnlineAsync(id, request);
    if (response.Status == 1)
    {
        return BadRequest(new ApiResponse<ClassStudentOnlineResponse>(response.Status, response.Message,response.Data)); 
    }

    return Ok(new ApiResponse<ClassStudentOnlineResponse>(response.Status, response.Message, response.Data));
}
    
[HttpDelete("{id?}")]
public async Task<IActionResult> DeleteDepartment(String id)
{
    var response = await _classStudentsOnlineService.DeleteClassStudentOnlineAsync(id);
    if (response.Status == 1)
    {
        return BadRequest(new ApiResponse<ClassStudentOnlineResponse>(response.Status, response.Message,response.Data)); 
    }

    return Ok(new ApiResponse<ClassStudentOnlineResponse>(response.Status, response.Message, response.Data));
}
}