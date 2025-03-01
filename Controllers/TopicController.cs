using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _service;
        public TopicController(ITopicService service)
        {
            _service = service;
        }
        [HttpGet]
        public Task<ApiResponse<List<TopicResponse>>> GetAll()
        {

            return _service.GetAll();
        }
        [HttpPost]
        public Task<ApiResponse<TopicResponse>> Create(TopicRequest request)
        {
            return _service.Create(request);
        }
        [HttpPut("{id}")]
        public Task<ApiResponse<TopicResponse>> Update(int id, TopicRequest request)
        {
            return _service.Update(id, request);
        }
        [HttpDelete("{id}")]
        public Task<ApiResponse<TopicResponse>> Delete(int id)
        {
            return _service.Delete(id);
        }
        [HttpGet("{id}")]
        public Task<ApiResponse<TopicResponse>> Search(int id)
        {
            return _service.Search(id);
        }
    }
}
