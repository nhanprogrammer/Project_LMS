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
    public async Task<ActionResult<ApiResponse<PaginatedResponse<LessonResponse>>>> GetAll([FromQuery] PaginationRequest request)
    {
        var result = await _lessonsService.GetLessonAsync(request);

        return Ok(new ApiResponse<PaginatedResponse<LessonResponse>>(0, "Success", result));
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

[HttpPut]
public async Task<IActionResult> UpdateLesson([FromBody] UpdateLessonRequest request)
{
    var response =   await _lessonsService.UpdateLessonAsync(request);
    if (response.Status == 1)
    {
        return BadRequest(new ApiResponse<LessonResponse>(response.Status, response.Message,response.Data)); 
    }

    return Ok(new ApiResponse<LessonResponse>(response.Status, response.Message, response.Data));
}

[HttpDelete]
public async Task<IActionResult> DeleteLesson(DeleteRequest ids)
{
    var response = await _lessonsService.DeleteLessonAsync(ids);
    if (response.Status == 1)
    {
        return BadRequest(new ApiResponse<LessonResponse>(response.Status, response.Message,response.Data)); 
    }

    return Ok(new ApiResponse<LessonResponse>(response.Status, response.Message, response.Data));
}


}