using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassStudentController : ControllerBase
    {
        private readonly IClassStudentsService _classStudentsService;

        public ClassStudentController(IClassStudentsService classStudentsService)
        {
            _classStudentsService = classStudentsService;
        }

        // GET: api/ClassStudent
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _classStudentsService.GetAllAsync();
            if (response.Status == 1)
            {
                return BadRequest(response); // Trả về lỗi nếu có
            }

            return Ok(response); // Trả về dữ liệu khi thành công
        }

        // GET: api/ClassStudent/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _classStudentsService.GetByIdAsync(id);
            if (response.Status == 1)
            {
                return NotFound(response); // Trả về lỗi nếu không tìm thấy
            }

            return Ok(response); // Trả về dữ liệu nếu tìm thấy
        }

        // POST: api/ClassStudent
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClassStudentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _classStudentsService.AddAsync(request);
            if (response.Status == 1)
            {
                return BadRequest(response); // Trả về lỗi nếu thêm mới không thành công
            }

            return CreatedAtAction(nameof(GetById), new { id = request.StudentId }, response); // Trả về dữ liệu khi thành công
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateClassStudentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            request.Id = id;
            var response = await _classStudentsService.UpdateAsync(request);

            // Trả về thông báo từ ApiResponse
            if (response.Status == 1)
            {
                return BadRequest(response); // Nếu có lỗi
            }

            return Ok(response); // Nếu thành công, trả về thông báo thành công
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _classStudentsService.DeleteAsync(id);

            if (response.Status == 1)
            {
                return BadRequest(response); // Trả về lỗi nếu có
            }

            return Ok(response); // Trả về thông báo thành công khi xóa
        }


    }
}
