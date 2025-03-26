using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestExamTypeController : ControllerBase
    {
        private readonly ITestExamTypeService _service;
        private readonly IAuthService _authService;

        public TestExamTypeController(ITestExamTypeService service, IAuthService authService)
        {
            _service = service;
            _authService = authService;
        }
        [HttpGet]
        public Task<ApiResponse<PaginatedResponse<TestExamTypeResponse>>> GetAll(
            [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
            [FromQuery, Range(1, int.MaxValue)] int pageSize = 10,
            [FromQuery] string? keyword = null)
        {
            return _service.GetAll(pageNumber, pageSize, keyword);
        }

        [HttpGet("coefficients")]
        public async Task<ActionResult<ApiResponse<List<int>>>> GetCoefficients()
        {
            try
            {
                var response = await _service.GetCoefficients();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy danh sách hệ số", ex.Message));
            }
        }
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TestExamTypeResponse>>> Create(TestExamTypeRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ApiResponse<string>(1, "Request body không được để trống", null));
            }

            var user = await _authService.GetUserAsync();
            if (user == null)
                return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

            var response = await _service.Create(request, user.Id);
            return response.Status == 0
                ? Ok(response)
                : BadRequest(new ApiResponse<string>(1, response.Message, null));

        }
        [HttpPut]
        public async Task<ActionResult<ApiResponse<TestExamTypeResponse>>> Update([FromBody] TestExamTypeRequest request)
        {
            try
            {
                if (request == null || request.Id == null || request.Id <= 0)
                {
                    return BadRequest(new ApiResponse<string>(1, "Request body hoặc Id không được để trống hoặc không hợp lệ", null));
                }

                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                var response = await _service.Update(request.Id.Value, request, user.Id);
                return new ApiResponse<TestExamTypeResponse>(0, "Cập nhật loại điểm thành công!", response.Data);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Đã xảy ra lỗi khi cập nhật loại điểm: {ex.Message}", null));
            }


        }

        [HttpDelete("{id}")]
        public Task<ApiResponse<TestExamTypeResponse>> Delete(int id)
        {
            return _service.Delete(id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TestExamTypeResponse>>> GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ApiResponse<string>(1, "Id không hợp lệ. Id phải lớn hơn 0.", null));
                }

                var response = await _service.GetById(id);
                if (response == null || response.Status == 1)
                {
                    return NotFound(new ApiResponse<TestExamTypeResponse>(1, "Không tìm thấy loại điểm với Id được cung cấp.", null));
                }

                return Ok(new ApiResponse<TestExamTypeResponse>(0, "Lấy loại điểm thành công!", response.Data));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Đã xảy ra lỗi khi lấy loại điểm: {ex.Message}", null));
            }
        }
    }
}
