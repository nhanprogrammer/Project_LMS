﻿using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;

namespace Project_LMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<LessonResponse>>>> GetAll(
        [FromQuery] string? keyword = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var response = await _lessonService.GetLessonAsync(keyword, pageNumber, pageSize);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> GetById(int id)
    {
        try
        {
            var result = await _lessonService.GetLessonByIdAsync(id);
            if (result.Data == null)
            {
                return NotFound(new ApiResponse<LessonResponse>(1, "Lesson not found", null));
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> Create([FromBody] CreateLessonRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<LessonResponse>(1, "Invalid request data", null));
            }

            var result = await _lessonService.CreateLessonAsync(request);
            
            if (result.Status != 0)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<LessonResponse>(1, $"Internal server error: {ex.Message}", null));
        }
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> Update([FromBody] CreateLessonRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<LessonResponse>(1, "Invalid request data", null));
            }

            if (request == null)
            {
                return BadRequest(new ApiResponse<LessonResponse>(1, "Request body cannot be null", null));
            }

            var result = await _lessonService.UpdateLessonAsync(request);
            
            if (result.Status != 0)
            {
                if (result.Message.Contains("not found"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<LessonResponse>(1, $"Internal server error: {ex.Message}", null));
        }
    }

    [HttpDelete]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteMultiple([FromBody] DeleteMultipleRequest request)
    {
        try
        {
            if (request?.Ids == null || !request.Ids.Any())
            {
                return BadRequest(new ApiResponse<bool>(1, "No IDs provided", false));
            }

            var result = await _lessonService.DeleteMultipleLessonsAsync(request.Ids);
            if (result.Status == 0)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<bool>(1, $"Error deleting lessons: {ex.Message}", false));
        }
    }
}