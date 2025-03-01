using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;

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
        public Task<ApiResponse<List<UserResponse>>> GetAll()
        {

            return _service.GetAll();
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
    }
}
