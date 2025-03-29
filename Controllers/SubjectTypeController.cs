using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectTypeController : ControllerBase
    {
        private readonly ISubjectTypeService _subjectTypeService;
        private readonly IAuthService _authService;

        public SubjectTypeController(ISubjectTypeService subjectTypeService, IAuthService authService)
        {
            _subjectTypeService = subjectTypeService;
            _authService = authService;
        }

        // [Authorize(Policy = "SUBJECT-TYPE-VIEW")]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<SubjectTypeResponse>>>> GetAll(
            [FromQuery] string? keyword = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                var response = await _subjectTypeService.GetAllSubjectTypesAsync(keyword, pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        // [Authorize(Policy = "SUBJECT-TYPE-VIEW")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<SubjectTypeResponse>>> GetById(int id)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

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

        // [Authorize(Policy = "SUBJECT-TYPE-INSERT")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<SubjectTypeResponse>>> Create([FromBody] SubjectTypeRequest request)
        {
            try
            {
                // Kiểm tra ModelState
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<SubjectTypeResponse>(1, "Dữ liệu không hợp lệ", null));
                }

                // Kiểm tra request
                if (request == null)
                {
                    return BadRequest(new ApiResponse<SubjectTypeResponse>(1, "Dữ liệu không được để trống", null));
                }

                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                var result = await _subjectTypeService.CreateSubjectTypeAsync(request);
                if (result.Status != 0) // Kiểm tra kết quả trả về
                {
                    return BadRequest(result);
                }

                return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        // [Authorize(Policy = "SUBJECT-TYPE-UPDATE")]
        [HttpPut()]
        public async Task<ActionResult<ApiResponse<SubjectTypeResponse>>> Update([FromBody] SubjectTypeRequest request)
        {
            try
            {
                // Kiểm tra ModelState
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<SubjectTypeResponse>(1, "Dữ liệu không hợp lệ", null));
                }

                // Kiểm tra request
                if (request == null)
                {
                    return BadRequest(new ApiResponse<SubjectTypeResponse>(1, "Dữ liệu không được để trống", null));
                }

                if (string.IsNullOrEmpty(request.Name))
                {
                    return BadRequest(new ApiResponse<SubjectTypeResponse>(1, "Tên loại môn học không được để trống", null));
                }

                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                var result = await _subjectTypeService.UpdateSubjectTypeAsync(request);
                if (result.Status != 0)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        // [Authorize(Policy = "SUBJECT-TYPE-DELETE")]
        [HttpDelete]
        public async Task<ActionResult<ApiResponse<bool>>> Delete([FromBody] DeleteMultipleRequest request)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

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