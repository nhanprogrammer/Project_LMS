using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;

namespace Project_LMS.Controllers;
[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentsService _departmentsService;

    public DepartmentController(IDepartmentsService departmentsService)
    {
        _departmentsService = departmentsService;
    }
    
    [HttpGet]
    public async Task<IActionResult>  GetAllDepartment()
    {
        var response = await _departmentsService.GetAllCoursesAsync();

        if (response.Status == 1)
        {
            return BadRequest(new ApiResponse<List<DepartmentResponse>>(response.Status, response.Message,response.Data)); 
        }

        return Ok(new ApiResponse<List<DepartmentResponse>>(response.Status, response.Message, response.Data));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentRequest request)
    {
        var response = await _departmentsService.CreateDepartmentAsync(request);

        if (response.Status == 1)
        {
            return BadRequest(new ApiResponse<DepartmentResponse>(response.Status, response.Message,response.Data)); 
        }

        return Ok(new ApiResponse<DepartmentResponse>(response.Status, response.Message, response.Data));
    }
    
    [HttpPut("{id?}")]
    public async Task<IActionResult> UpdateDepartment(String id, [FromBody] UpdateDepartmentRequest request)
    {
        var response =   await _departmentsService.UpdateDepartmentAsync(id, request);
        if (response.Status == 1)
        {
            return BadRequest(new ApiResponse<DepartmentResponse>(response.Status, response.Message,response.Data)); 
        }

        return Ok(new ApiResponse<DepartmentResponse>(response.Status, response.Message, response.Data));
    }
    
    [HttpDelete("{id?}")]
    public async Task<IActionResult> DeleteDepartment(String id)
    {
        var response = await _departmentsService.DeleteDepartmentAsync(id);
        if (response.Status == 1)
        {
            return BadRequest(new ApiResponse<DepartmentResponse>(response.Status, response.Message,response.Data)); 
        }

        return Ok(new ApiResponse<DepartmentResponse>(response.Status, response.Message, response.Data));
    }
}