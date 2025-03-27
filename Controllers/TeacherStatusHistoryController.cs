using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherStatusHistoryController : ControllerBase
    {
        private readonly ITeacherStatusHistoryService _service;

        public TeacherStatusHistoryController(ITeacherStatusHistoryService service)
        {
            _service = service;
        }

        [HttpPost("retirement")]
        public async Task<IActionResult> AddRetirement( TeacherStatusHistoryRequest request)
        {
            var result =await _service.AddAsync("Nghỉ hưu",request);
            return Ok(result);
        }
        [HttpPost("resignation")]
        public async Task<IActionResult> AddResignation([FromBody] TeacherStatusHistoryRequest request)
        {
            var result =await _service.AddAsync("Đã nghỉ việc", request);
            return Ok(result);
        }
        [HttpPost("suspension")]
        public async Task<IActionResult> AddSuspension([FromBody] TeacherStatusHistoryRequest request)
        {
            var result =await _service.AddAsync("Tạm nghỉ",request);
            return Ok(result);
        }
    }
}
