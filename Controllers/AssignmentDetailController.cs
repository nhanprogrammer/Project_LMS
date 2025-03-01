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
    public class AssignmentDetailController : ControllerBase
    {
        private readonly IAssignmentDetailsService _service;

        public AssignmentDetailController(IAssignmentDetailsService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AssignmentDetailResponse>>> GetById(int id)
        {
            try
            {
                var result = await _service.GetAssignmentDetailById(id);
                if (result == null)
                {
                    return NotFound(new ApiResponse<AssignmentDetailResponse>(1, "Assignment detail not found", null));
                }
                return Ok(new ApiResponse<AssignmentDetailResponse>(0, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssignmentDetailResponse>>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllAssignmentDetails();
                return Ok(new ApiResponse<IEnumerable<AssignmentDetailResponse>>(0, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CreateAssignmentDetailRequest>>> Add([FromBody] CreateAssignmentDetailRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResponse<CreateAssignmentDetailRequest>(1, "Invalid request", null));
                }

                return Ok(new ApiResponse<CreateAssignmentDetailRequest>(0, "Assignment detail added successfully", request));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UpdateAssignmentDetailRequest>>> Update(int id, [FromBody] UpdateAssignmentDetailRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResponse<UpdateAssignmentDetailRequest>(1, "Invalid request", null));
                }

                return Ok(new ApiResponse<UpdateAssignmentDetailRequest>(0, "Assignment detail updated successfully", request));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteAssignmentDetail(id);
                if (!result)
                {
                    return NotFound(new ApiResponse<string>(1, "Assignment detail not found", null));
                }

                return Ok(new ApiResponse<string>(0, "Assignment detail deleted successfully", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }
    }
}
