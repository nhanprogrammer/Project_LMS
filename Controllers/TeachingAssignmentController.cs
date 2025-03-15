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

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<TeachingAssignmentResponse>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAll(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeachingAssignmentResponse>> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result == null)
            {
                Console.WriteLine($"API GetById: Không tìm thấy TeachingAssignment với ID: {id}");
                return NotFound(new { Message = $"Không tìm thấy phân công giảng dạy với ID: {id}" });
            }
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TeachingAssignmentRequest request)
        {
            try
            {
                Console.WriteLine($"Bắt đầu tạo phân công: UserId={request.UserId}, ClassId={request.ClassId}, SubjectId={request.SubjectId}");

                var response = await _service.Create(request);
                if (response != null)
                {
                    return Ok(new { Message = "Phân công giảng dạy đã được tạo!", Data = response });
                }
                return BadRequest(new { Message = "Tạo phân công thất bại!" });
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Lỗi khi lưu vào database: {dbEx.InnerException?.Message ?? dbEx.Message}");
                return StatusCode(500, new
                {
                    Message = "Lỗi khi lưu dữ liệu vào database.",
                    Error = dbEx.InnerException?.Message ?? dbEx.Message
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi không xác định: {ex.Message}");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi.", Error = ex.Message });
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TeachingAssignmentRequest request)
        {
            try
            {
                var success = await _service.Update(id, request);
                if (success)
                {
                    return Ok(new { Message = "Cập nhật thành công!" });
                }
                return NotFound(new { Message = "Không tìm thấy phân công giảng dạy!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật phân công giảng dạy (ID: {id}): {ex.Message}");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi cập nhật phân công giảng dạy.", Error = ex.Message });
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.Delete(id);
            return success
                ? Ok(new { Message = "Xóa thành công!" })
                : NotFound(new { Message = "Không tìm thấy phân công giảng dạy!" });
        }
    }

}
