using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Services;

namespace Project_LMS.Controllers;
[ApiController]
[Route("api/[controller]")]
public class LessonController : ControllerBase
{
private readonly ILessonsService _lessonsService;

public LessonController(ILessonsService lessonsService)
{
    _lessonsService = lessonsService;
}

[HttpGet]
public async Task<IActionResult>  GetAllLessonAsync()
{
    var response = await _lessonsService.GetAllLessonAsync();

    if (response.Status == 1)
    {
        return BadRequest(new ApiResponse<List<LessonResponse>>(response.Status, response.Message,response.Data)); 
    }

    return Ok(new ApiResponse<List<LessonResponse>>(response.Status, response.Message, response.Data));
}

[HttpPost]
public async Task<IActionResult> CreateFavouriteAsync([FromBody] CreateLessonRequest request)
{
    var response = await _lessonsService.CreateLessonAsync(request);

    if (response.Status == 1)
    {
        return BadRequest(new ApiResponse<LessonResponse>(response.Status, response.Message,response.Data)); 
    }

    return Ok(new ApiResponse<LessonResponse>(response.Status, response.Message, response.Data));
}

[HttpPut("{id?}")]
public async Task<IActionResult> UpdateFavourite(String id, [FromBody] UpdateLessonRequest request)
{
    var response =   await _lessonsService.UpdateLessonAsync(id, request);
    if (response.Status == 1)
    {
        return BadRequest(new ApiResponse<LessonResponse>(response.Status, response.Message,response.Data)); 
    }

    return Ok(new ApiResponse<LessonResponse>(response.Status, response.Message, response.Data));
}

[HttpDelete("{id?}")]
public async Task<IActionResult> DeleteDepartment(String id)
{
    var response = await _lessonsService.DeleteLessonAsync(id);
    if (response.Status == 1)
    {
        return BadRequest(new ApiResponse<LessonResponse>(response.Status, response.Message,response.Data)); 
    }

    return Ok(new ApiResponse<LessonResponse>(response.Status, response.Message, response.Data));
}


}