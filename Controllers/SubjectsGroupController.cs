// Controllers/SubjectsGroupController.cs
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectsGroupController : ControllerBase
    {
        private readonly ISubjectsGroupService _subjectsGroupService;

        public SubjectsGroupController(ISubjectsGroupService subjectsGroupService)
        {
            _subjectsGroupService = subjectsGroupService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<SubjectsGroupResponse>>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _subjectsGroupService.GetAllSubjectsGroupsAsync(pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<SubjectsGroupResponse>>> GetById(int id)
        {
            try
            {
                var result = await _subjectsGroupService.GetSubjectsGroupByIdAsync(id);
                if (result.Data == null)
                {
                    return NotFound(new ApiResponse<SubjectsGroupResponse>(1, "SubjectsGroup not found", null));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<SubjectsGroupResponse>>> Create([FromBody] SubjectsGroupRequest request)
        {
            try
            {
                var result = await _subjectsGroupService.CreateSubjectsGroupAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<SubjectsGroupResponse>>> Update(int id, [FromBody] SubjectsGroupRequest request)
        {
            try
            {
                var result = await _subjectsGroupService.UpdateSubjectsGroupAsync(id, request);
                if (result.Data == null)
                {
                    return NotFound(new ApiResponse<SubjectsGroupResponse>(1, "SubjectsGroup not found", null));
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
                var result = await _subjectsGroupService.DeleteSubjectsGroupAsync(id);
                if (!result.Data)
                {
                    return NotFound(new ApiResponse<bool>(1, "SubjectsGroup not found", false));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }
    }
}