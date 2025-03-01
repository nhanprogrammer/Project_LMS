using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Services;
using System.Threading.Tasks;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;

        public ClassController(IClassService classService)
        {
            _classService = classService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> GetAllClasses()
        {
            return Ok(await _classService.GetAllClassesAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> GetClassById(int id)
        {
            return Ok(await _classService.GetClassByIdAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> CreateClass(CreateClassRequest request)
        {
            return Ok(await _classService.CreateClassAsync(request));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClass(int id, [FromBody] UpdateClassRequest request)
        {
            var response = await _classService.UpdateClassAsync(id, request);
            return StatusCode(response.Status == 0 ? 200 : 400, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var response = await _classService.DeleteClassAsync(id);
            return StatusCode(response.Status == 0 ? 200 : 400, response);
        }

    }
}
