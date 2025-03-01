using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WardController : ControllerBase
    {
        private readonly IWardService _service;
        public WardController(IWardService service)
        {
            _service = service;
        }
        [HttpGet]
        public Task<ApiResponse<List<WardResponse>>> getAll()
        {
            return _service.GetAll();
        }
        [HttpPost]
        public Task<ApiResponse<WardResponse>> Create(WardRequest request)
        {
            return _service.Create(request);
        }
        [HttpPut("{id}")]
        public Task<ApiResponse<WardResponse>> Update(int id, WardRequest request)
        {
            return _service.Update(id, request);
        }
        [HttpDelete("{id}")]
        public Task<ApiResponse<WardResponse>> Delete(int id)
        {
            return _service.Delete(id);
        }
        [HttpGet("{id}")]
        public Task<ApiResponse<WardResponse>> Search(int id)
        {
            return _service.Search(id);
        }
    }
}
