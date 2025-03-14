
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Services;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service)
        {
            _service = service;
        }
        [HttpGet]
        public Task<ApiResponse<PaginatedResponse<object>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return _service.GetAll(pageNumber, pageSize);
        }


        [HttpPost]
        public Task<ApiResponse<UserResponse>> Create(UserRequest request)
        {
            return _service.Create(request);
        }
        [HttpPut("{id}")]
        public Task<ApiResponse<UserResponse>> Update(int id, UserRequest request)
        {
            return _service.Update(id, request);
        }
        [HttpDelete("{id}")]
        public Task<ApiResponse<UserResponse>> Delete(int id)
        {
            return _service.Delete(id);
        }
        [HttpGet("{id}")]
        public Task<ApiResponse<UserResponse>> Search(int id)
        {
            return _service.Search(id);
        }
        [HttpGet("ByIds")]
        public Task<ApiResponse<List<UserResponse>>> GetAllByIds([FromQuery] List<int> ids, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return _service.GetAllByIds(ids, pageNumber, pageSize);
        }
        [HttpGet("export-users")]
        public async Task<IActionResult> ExportUsers()
        {
            var result = await _service.ExportUsersToExcel();
            return Ok(result); // Trả về ApiResponse<byte[]> trong body
        }
        [HttpGet("checkuser/{name}")]
        public async Task<IActionResult> CheckUser(string name)
        {
            var result = await _service.CheckUser(name);
            return Ok(result);
        }       [HttpGet("forgotpassword")]
        [Authorize]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            var result = await _service.ForgotPassword(request);
            return Ok(result);
        }

    }
}
