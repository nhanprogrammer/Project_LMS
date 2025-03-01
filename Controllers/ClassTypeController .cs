using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Models;
using Project_LMS.Services;
using System.Threading.Tasks;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassTypeController : ControllerBase
    {
        private readonly IClassTypeService _classTypeService;

        public ClassTypeController(IClassTypeService classTypeService)
        {
            _classTypeService = classTypeService;
        }

        // GET: api/ClassType
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _classTypeService.GetAllAsync();
            if (response.Status == 1)
            {
                return BadRequest(response); // Trả về lỗi nếu có
            }

            return Ok(response); // Trả về dữ liệu khi thành công
        }

        // GET: api/ClassType/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _classTypeService.GetByIdAsync(id);
            if (response.Status == 1)
            {
                return NotFound(response); // Trả về lỗi nếu không tìm thấy
            }

            return Ok(response); // Trả về dữ liệu khi thành công
        }

        // POST: api/ClassType
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClassType request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _classTypeService.AddAsync(request);
            if (response.Status == 1)
            {
                return BadRequest(response); // Trả về lỗi nếu thêm mới không thành công
            }

            return CreatedAtAction(nameof(GetById), new { id = request.Id }, response); // Trả về dữ liệu khi thành công
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClassType request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            request.Id = id;
            var response = await _classTypeService.UpdateAsync(request);

            // Trả về thông báo từ ApiResponse
            if (response.Status == 1)
            {
                return BadRequest(response); // Nếu có lỗi
            }

            return Ok(response); // Nếu thành công, trả về thông báo thành công
        }


        // DELETE: api/ClassType/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _classTypeService.DeleteAsync(id);
            if (response.Status == 1)
            {
                return BadRequest(response); // Trả về lỗi nếu xóa không thành công
            }

            return NoContent(); // Trả về NoContent khi thành công
        }
    }
}
