
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingRankController : ControllerBase
    {
        private readonly ITrainingRankService _service;
        public TrainingRankController(ITrainingRankService service)
        {
            _service = service;
        }
        [HttpGet]
        public Task<ApiResponse<List<TrainingRankResponse>>> getAll()
        {
            return _service.GetAll();
        }
        [HttpPost]
        public Task<ApiResponse<TrainingRankResponse>> Create(TrainingRankRequest request)
        {
            return _service.Create(request);
        }
        [HttpPut("{id}")]
        public Task<ApiResponse<TrainingRankResponse>> Update(int id, TrainingRankRequest request)
        {
            return _service.Update(id, request);
        }
        [HttpDelete("{id}")]
        public Task<ApiResponse<TrainingRankResponse>> Delete(int id)
        {
            return _service.Delete(id);
        }
        [HttpGet("{id}")]
        public Task<ApiResponse<TrainingRankResponse>> Search(int id)
        {
            return _service.Search(id);
        }
    }
}
