using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestExamTypeController : ControllerBase
    {
        private readonly ITestExamTypeService _service;
        public TestExamTypeController(ITestExamTypeService service)
        {
            _service = service;
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
        public Task<ApiResponse<TestExamTypeResponse>> Create(TestExamTypeRequest request)
        {
            return _service.Create(request);
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

                var response = await _service.Update(request.Id.Value, request);
                if (response == null)
                {
                    return NotFound(new ApiResponse<TestExamTypeResponse>(1, "Không tìm thấy loại điểm cần cập nhật", null));
                }

                return Ok(new ApiResponse<TestExamTypeResponse>(0, "Cập nhật loại điểm thành công", response.Data));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi cập nhật loại điểm", ex.Message));
            }
        }
        [HttpDelete("{id}")]
        public Task<ApiResponse<TestExamTypeResponse>> Delete(int id)
        {
            return _service.Delete(id);
        }
    }
}
