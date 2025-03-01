using Microsoft.AspNetCore.Mvc;
using Project_LMS.Interfaces.Services;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Services;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionsService _questionService;

        public QuestionController(IQuestionsService questionService)
        {
            _questionService = questionService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<QuestionResponse>>>> GetAll()
        {
            try
            {
                var questions = await _questionService.GetAllAsync();
                return Ok(new ApiResponse<IEnumerable<QuestionResponse>>(1, "Lấy danh sách câu hỏi thành công", questions));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy danh sách câu hỏi", ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<QuestionResponse>>> GetById(int id)
        {
            try
            {
                var question = await _questionService.GetByIdAsync(id);
                if (question == null)
                {
                    return NotFound(new ApiResponse<QuestionResponse>(0, "Không tìm thấy câu hỏi"));
                }
                return Ok(new ApiResponse<QuestionResponse>(1, "Lấy câu hỏi thành công", question));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy câu hỏi", ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<QuestionResponse>>> Create(QuestionRequest request)
        {
            try
            {
                var question = await _questionService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = question.Id }, new ApiResponse<QuestionResponse>(1, "Tạo câu hỏi thành công", question));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<List<ValidationError>>(400, "Validation failed.", ex.Errors));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi tạo câu hỏi", ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<QuestionResponse>>> Update(int id, QuestionRequest request)
        {
            try
            {
                var question = await _questionService.UpdateAsync(id, request);
                if (question == null)
                {
                    return NotFound(new ApiResponse<QuestionResponse>(0, "Không tìm thấy câu hỏi"));
                }
                return Ok(new ApiResponse<QuestionResponse>(1, "Cập nhật câu hỏi thành công", question));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<List<ValidationError>>(400, "Validation failed.", ex.Errors));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi cập nhật câu hỏi", ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<QuestionResponse>>> Delete(int id)
        {
            try
            {
                var question = await _questionService.DeleteAsync(id);
                if (question == null)
                {
                    return NotFound(new ApiResponse<QuestionResponse>(0, "Không tìm thấy câu hỏi"));
                }
                return Ok(new ApiResponse<QuestionResponse>(1, "Xóa câu hỏi thành công", question));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi xóa câu hỏi", ex.Message));
            }
        }
    }
}