using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentsService _service;

        public AssignmentController(IAssignmentsService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AssignmentsResponse>>> GetById(int id)
        {
            try
            {
                var result = await _service.GetAssignmentById(id);
                if (result == null)
                {
                    return NotFound(new ApiResponse<AssignmentsResponse>(1, "Assignment not found"));
                }
                return Ok(new ApiResponse<AssignmentsResponse>(0, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssignmentsResponse>>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllAssignments();
                return Ok(new ApiResponse<IEnumerable<AssignmentsResponse>>(0, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromBody] CreateAssignmentRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResponse<string>(1, "Invalid request data"));
                }

                await _service.AddAssignment(request);
                return Ok(new ApiResponse<string>(0, "Assignment created successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(int id, [FromBody] UpdateAssignmentRequest request)
        {
            try
            {
                var existingAssignment = await _service.GetAssignmentById(id);
                if (existingAssignment == null)
                {
                    return NotFound(new ApiResponse<string>(1, "Assignment not found"));
                }

                await _service.UpdateAssignment(id, request);
                return Ok(new ApiResponse<string>(0, "Assignment updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteAssignment(id);
                if (!deleted)
                {
                    return NotFound(new ApiResponse<string>(1, "Assignment not found"));
                }
                return Ok(new ApiResponse<string>(0, "Assignment deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}"));
            }
        }
    }
}
