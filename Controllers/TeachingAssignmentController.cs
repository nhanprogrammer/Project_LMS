using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachingAssignmentController : ControllerBase
    {
        private readonly ITeachingAssignmentService _service;

        public TeachingAssignmentController(ITeachingAssignmentService service)
        {
            _service = service;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? academicYearId = null,
            [FromQuery] int? subjectGroupId = null)
        {
            var result = await _service.GetAll(pageNumber, pageSize, academicYearId, subjectGroupId);
            if (result.TotalItems == 0)
            {
                return Ok(new
                {
                    message = "Không tìm thấy dữ liệu",
                    data = result
                });
            }

            return Ok(new ApiResponse<PaginatedResponse<TeachingAssignmentResponse>>(0, "Lấy dữ liệu thành công!",
                result));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result == null)
            {
                Console.WriteLine($"API GetById: Không tìm thấy TeachingAssignment với ID: {id}");
                return NotFound(new ApiResponse<object>(1, $"Không tìm thấy phân công giảng dạy với ID: {id}"));
            }

            return Ok(new ApiResponse<object>(0, "Success", result));
        }

        [HttpGet("getuser/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var result = await _service.GetByUserId(userId);
            if (result == null)
            {
                Console.WriteLine($"API GetUserById: Không tìm thấy TeachingAssignment với UserId: {userId}");
                return NotFound(new ApiResponse<object>(1, $"Không tìm thấy phân công giảng dạy với UserId: {userId}"));
            }

            return Ok(new ApiResponse<object>(0, "Success", result));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TeachingAssignmentRequestCreate request)
        {
            try
            {
                //Console.WriteLine($"Bắt đầu tạo phân công: UserId={request.UserId}, ClassId={request.ClassId}, SubjectId={request.SubjectId}");

                var response = await _service.Create(request);
                if (response != null)
                {
                    return Ok(new ApiResponse<object>(0, "Phân công giảng dạy đã được tạo!", response));
                }

                return BadRequest(new ApiResponse<object>(1, "Tạo phân công thất bại!"));
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Lỗi khi lưu vào database: {dbEx.InnerException?.Message ?? dbEx.Message}");
                return StatusCode(500,
                    new ApiResponse<object>(1, "Lỗi khi lưu dữ liệu vào database.",
                        dbEx.InnerException?.Message ?? dbEx.Message));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi không xác định: {ex.Message}");
                return StatusCode(500, new ApiResponse<object>(1, "Đã xảy ra lỗi.", ex.Message));
            }
        }

        [HttpPut("user/{userId}")]
        public async Task<IActionResult> UpdateByUserId(int userId, [FromBody] TeachingAssignmentRequest request)
        {
            var result = await _service.UpdateByUserId(userId, request);
            return result != null
                ? Ok(new ApiResponse<TeachingAssignmentResponse>(0, "Cập nhật thành công!", result))
                : NotFound(new ApiResponse<object>(1, "Không tìm thấy phân công cho người dùng!"));
        }


        [HttpDelete("deleteList")]
        public async Task<IActionResult> Delete([FromBody] DeleteRequest request)
        {
            var success = await _service.Delete(request.ids);
            return success
                ? Ok(new ApiResponse<object>(0, "Xóa danh sách thành công!"))
                : NotFound(new ApiResponse<object>(1, "Không tìm thấy các phân công giảng dạy!"));
        }
    }
}