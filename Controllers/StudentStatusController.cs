using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentStatusController : ControllerBase
    {
        private readonly IStudentStatusService _service;
        public StudentStatusController(IStudentStatusService service)
        {
            _service = service;
        }

        [HttpGet]
        public async  Task<ApiResponse<List<StudentStatusResponse>>> GetAll()
        {
            return await _service.GetAll();
        }[HttpGet("Search")]
        public async  Task<ApiResponse<StudentStatusResponse>> Search([FromQuery] int id)
        {
            return await _service.Search(id);
        }
    }
}
