using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTrainingRankController : ControllerBase
    {
        private readonly IUserTrainingRankService _service;
        public UserTrainingRankController(IUserTrainingRankService service)
        {
            _service = service;
        }
        [HttpGet]
        public Task<ApiResponse<List<UserTrainingRankResponse>>> GetAll()
        {

            return _service.GetAll();
        }
        [HttpPost]
        public Task<ApiResponse<UserTrainingRankResponse>> Create(UserTrainingRankRequest request)
        {
            return _service.Create(request);
        }
        [HttpPut("{id}")]
        public Task<ApiResponse<UserTrainingRankResponse>> Update(int id, UserTrainingRankRequest request)
        {
            return _service.Update(id, request);
        }
        [HttpDelete("{id}")]
        public Task<ApiResponse<UserTrainingRankResponse>> Delete(int id)
        {
            return _service.Delete(id);
        }
        [HttpGet("{id}")]
        public Task<ApiResponse<UserTrainingRankResponse>> Search(int id)
        {
            return _service.Search(id);
        }
    }
}
