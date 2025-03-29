﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers;

[Authorize(Policy = "TEACHER")]
[Route("api/[controller]")]
[ApiController]
public class TeacherTestExamController : ControllerBase
{
    private readonly ITeacherTestExamService _teacherTestExamService;


    public TeacherTestExamController(ITeacherTestExamService teacherTestExamService)
    {
        _teacherTestExamService = teacherTestExamService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDisciplinesAsync(   int? pageNumber,
        int? pageSize,
        string? sortDirection,
        string? topicName,
        string? subjectName,
        string? department,
        string? startDate)
    {
        var response = await _teacherTestExamService.GetTeacherTestExamAsync(
            pageNumber, pageSize, sortDirection, topicName, subjectName, department, startDate);

        if (response.Status == 1)
        {
            return BadRequest(
                new ApiResponse<PaginatedResponse<TeacherTestExamResponse>>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<PaginatedResponse<TeacherTestExamResponse>>(response.Status, response.Message, response.Data));
    }

    [HttpGet("{id?}")]
    public async Task<IActionResult> UpdateDiscipline(int id)
    {
        var response = await _teacherTestExamService.GetTeacherTestExamById(id);

        if (response.Status == 1)
        {
            return BadRequest(new ApiResponse<Object>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<Object>(response.Status, response.Message, response.Data));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Object>>> Create([FromBody] TeacherTestExamRequest request)
    {
        var response = await _teacherTestExamService.CreateTeacherTestExamAsync(request);

        if (response.Status == 1)
        {
            return BadRequest(
                new ApiResponse<Object>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<Object>(response.Status, response.Message, response.Data));
    }
    
    [HttpPut]
    public async Task<ActionResult<ApiResponse<Object>>> Update([FromBody] TeacherTestExamRequest request)
    {
        var response = await _teacherTestExamService.UpdateTeacherTestExamAsync(request);

        if (response.Status == 1)
        {
            return BadRequest(
                new ApiResponse<Object>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<Object>(response.Status, response.Message, response.Data));
    }
    
    [HttpGet("Filter")]
    public async Task<ActionResult<ApiResponse<Object>>> FilterClass([FromQuery] int departmentId )
    {
        var response = await _teacherTestExamService.GetFilterClass(departmentId);

        if (response.Status == 1)
        {
            return BadRequest(
                new ApiResponse<Object>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<Object>(response.Status, response.Message, response.Data));
    }
    
    
    [HttpDelete("{id?}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var response = await _teacherTestExamService.DeleteTeacherTestExamById(id);
        if (response.Status == 1)
        {
            return BadRequest(new ApiResponse<Object>(response.Status, response.Message, response.Data));
        }

        return Ok(new ApiResponse<Object>(response.Status, response.Message, response.Data));
    }
}