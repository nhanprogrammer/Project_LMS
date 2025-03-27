using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [Authorize(Policy = "DATA-MNG-VIEW")]
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        private readonly IAuthService _authService;

        public SubjectController(ISubjectService subjectService, IAuthService authService)
        {
            _subjectService = subjectService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<SubjectResponse>>>> GetAll(
            [FromQuery] string? keyword = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                var response = await _subjectService.GetAllSubjectsAsync(keyword, pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Lỗi hệ thống: {ex.Message}", null));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<SubjectResponse>>> GetById(int id)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                var result = await _subjectService.GetSubjectByIdAsync(id);
                if (result.Data == null)
                {
                    return NotFound(new ApiResponse<SubjectResponse>(1, "Không tìm thấy môn học", null));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Lỗi hệ thống: {ex.Message}", null));
            }
        }

        [Authorize(Policy = "DATA-MNG-INSERT")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<SubjectResponse>>> Create([FromBody] SubjectRequest request)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<SubjectResponse>(1, "Dữ liệu không hợp lệ", null));
                }

                var result = await _subjectService.CreateSubjectAsync(request);
                if (result.Status != 0)
                {
                    return BadRequest(result);
                }

                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<SubjectResponse>(1, $"Lỗi hệ thống: {ex.Message}", null));
            }
        }

        [Authorize(Policy = "DATA-MNG-UPDATE")]
        [HttpPut]
        public async Task<ActionResult<ApiResponse<SubjectResponse>>> Update([FromBody] SubjectRequest request)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<SubjectResponse>(1, "Dữ liệu không hợp lệ", null));
                }

                if (request == null)
                {
                    return BadRequest(new ApiResponse<SubjectResponse>(1, "Dữ liệu không được để trống", null));
                }

                if (string.IsNullOrEmpty(request.SubjectCode))
                {
                    return BadRequest(new ApiResponse<SubjectResponse>(1, "Mã môn học không được để trống", null));
                }

                var result = await _subjectService.UpdateSubjectAsync(request);
                if (result.Status != 0)
                {
                    if (result.Message.Contains("not found"))
                    {
                        return NotFound(result);
                    }
                    if (result.Message.Contains("already exists"))
                    {
                        return BadRequest(result);
                    }
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<SubjectResponse>(1, $"Lỗi hệ thống: {ex.Message}", null));
            }
        }

        [Authorize(Policy = "DATA-MNG-DELETE")]
        [HttpDelete]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteMultiple([FromBody] DeleteMultipleRequest request)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                if (request?.Ids == null || !request.Ids.Any())
                {
                    return BadRequest(new ApiResponse<bool>(1, "Chưa chọn môn học để xóa", false));
                }

                var result = await _subjectService.DeleteMultipleSubjectsAsync(request.Ids);
                if (result.Status == 0)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>(1, $"Lỗi khi xóa môn học: {ex.Message}", false));
            }
        }

        [HttpGet("search-subject")]
        public async Task<IActionResult> getSubjectByUserId([FromQuery] int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new ApiResponse<object>(1, "UserId không hợp lệ."));
            }

            var result = await _subjectService.getSubjectByUserId(userId);
            if (!result.Any() || result == null)
                return Ok(new ApiResponse<object>(1, "Giảng viên chưa có môn học nào!", null));
            return Ok(new ApiResponse<object>(0, "Lấy danh sách môn học thành công!", result));
        }
        
        [HttpGet("by-subject-group/{subjectGroupId}")]
        public async Task<IActionResult> GetSubjectsBySubjectGroupId(int subjectGroupId)
        {
            if (subjectGroupId <= 0)
            {
                return BadRequest(new ApiResponse<object>(1, "SubjectGroupId không hợp lệ."));
            }

            var result = await _subjectService.GetSubjectsBySubjectGroupIdAsync(subjectGroupId);

            if (result == null || !result.Any())
            {
                return Ok(new ApiResponse<object>(1, "Không có môn học nào thuộc tổ bộ môn này!", null));
            }

            return Ok(new ApiResponse<object>(0, "Lấy danh sách môn học thành công!", result));
        }
    }
}