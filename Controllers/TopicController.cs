using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTopics([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _topicService.GetAllTopicsAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTopicById(int id)
        {
            var result = await _topicService.GetTopicByIdAsync(id);
            if (result.Data == null)
            {
                // Nếu topic không tồn tại
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTopic([FromBody] CreateTopicRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _topicService.CreateTopicAsync(request);
            // 201 - Created nếu muốn chuẩn REST
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTopic([FromBody] UpdateTopicRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _topicService.UpdateTopicAsync(request);
            if (result.Status == 1)
            {
                // Nếu topic không tồn tại
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            var result = await _topicService.DeleteTopicAsync(id);
            if (result.Status == 1)
            {
                // Nếu topic không tồn tại
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchTopics([FromQuery] string? keyword)
        {
            var result = await _topicService.SearchTopicsAsync(keyword);
            if (result.Status == 1)
            {
                // Nếu không tìm thấy topic
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}