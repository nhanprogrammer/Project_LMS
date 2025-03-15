using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            var response = await _service.Create(request);
            return response != null
                ? Ok(new { Message = "Phân công giảng dạy đã được tạo!", Data = response })
                : BadRequest(new { Message = "Tạo phân công thất bại!" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TeachingAssignmentRequest request)
        {
            var success = await _service.Update(id, request);
            return success
                ? Ok(new { Message = "Cập nhật thành công!" })
                : NotFound(new { Message = "Không tìm thấy phân công giảng dạy!" });
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
