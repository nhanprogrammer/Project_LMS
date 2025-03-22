// Controllers/SubjectTypeController.cs
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectTypeController : ControllerBase
    {
        private readonly ISubjectTypeService _subjectTypeService;

        public SubjectTypeController(ISubjectTypeService subjectTypeService)
        {
            _subjectTypeService = subjectTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<SubjectTypeResponse>>>> GetAll(
            [FromQuery] string? keyword = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _subjectTypeService.GetAllSubjectTypesAsync(keyword, pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<SubjectTypeResponse>>> GetById(int id)
        {
            try
            {
                var result = await _subjectTypeService.GetSubjectTypeByIdAsync(id);
                if (result.Data == null)
                {
                    return NotFound(new ApiResponse<SubjectTypeResponse>(1, "SubjectType not found", null));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<SubjectTypeResponse>>> Create([FromBody] SubjectTypeRequest request)
        {
            try
            {
                var result = await _subjectTypeService.CreateSubjectTypeAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpPut()]
        public async Task<ActionResult<ApiResponse<SubjectTypeResponse>>> Update([FromBody] SubjectTypeRequest request)
        {
            try
            {
                var result = await _subjectTypeService.UpdateSubjectTypeAsync(request);
                if (result.Data == null)
                {
                    return NotFound(new ApiResponse<SubjectTypeResponse>(1, "SubjectType not found", null));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpDelete]
        public async Task<ActionResult<ApiResponse<bool>>> Delete([FromBody] DeleteMultipleRequest request)
        {
            try
            {
                if (request?.Ids == null || !request.Ids.Any())
                {
                    return BadRequest(new ApiResponse<bool>(1, "No IDs provided", false));
                }

                var result = await _subjectTypeService.DeleteSubjectTypeAsync(request.Ids);
                if (result.Status == 0)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>(1, $"Error deleting subject types: {ex.Message}", false));
            }
        }
    }
}