using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Project_LMS.Interfaces.Services;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

[Route("api/[controller]")]
[ApiController]
public class TopicController : ControllerBase
{
    private readonly ITopicService _service;

    public TopicController(ITopicService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<TopicResponse>>>> GetAll([FromQuery] string? keyword, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var response = await _service.GetAllTopicsAsync(keyword, pageNumber, pageSize);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TopicResponse>>> GetById(int id)
    {
        try
        {
            var result = await _service.GetTopicByIdAsync(id);
            if (result.Data == null)
            {
                return NotFound(new ApiResponse<TopicResponse>(1, "Topic not found", null));
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TopicResponse>>> Create([FromBody] TopicRequest request)
    {
        try
        {
            var result = await _service.CreateTopicAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TopicResponse>>> Update(int id, [FromBody] TopicRequest request)
    {
        try
        {
            var result = await _service.UpdateTopicAsync(id, request);
            if (result.Data == null)
            {
                return NotFound(new ApiResponse<TopicResponse>(1, "Topic not found", null));
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var result = await _service.DeleteTopicAsync(id);
            if (!result.Data)
            {
                return NotFound(new ApiResponse<bool>(1, "Topic not found", false));
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
        }
    }
}