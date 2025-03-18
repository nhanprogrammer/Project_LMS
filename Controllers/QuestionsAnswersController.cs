using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsAnswersController : ControllerBase
    {
        private readonly IQuestionsAnswersService _questionsAnswersService;

        public QuestionsAnswersController(IQuestionsAnswersService questionsAnswersService)
        {
            _questionsAnswersService = questionsAnswersService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequest request)
        {
            var result = await _questionsAnswersService.GetAllAsync(request);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _questionsAnswersService.GetByIdAsync(id);
            if (result.Data == null)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateQuestionsAnswerRequest request)
        {
            var result = await _questionsAnswersService.AddAsync(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateQuestionsAnswerRequest request,
            [FromQuery] int? newTopicId = null)
        {
            var result = await _questionsAnswersService.UpdateAsync(request, newTopicId);
            if (result.Data == null)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _questionsAnswersService.DeleteAsync(id);
            if (!result.Data)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("topic/{topicId:int}")]
        public async Task<IActionResult> GetAllQuestionAnswerByTopicId(int topicId)
        {
            var result = await _questionsAnswersService.GetAllQuestionAnswerByTopicIdAsync(topicId);
            if (result.Status == 1)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}