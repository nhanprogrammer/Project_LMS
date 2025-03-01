using Microsoft.AspNetCore.Mvc;
using Project_LMS.Interfaces.Services;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsAnswerTopicViewController : ControllerBase
    {
        private readonly IQuestionsAnswerTopicViewService _questionsAnswerTopicViewService;

        public QuestionsAnswerTopicViewController(IQuestionsAnswerTopicViewService questionsAnswerTopicViewService)
        {
            _questionsAnswerTopicViewService = questionsAnswerTopicViewService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<QuestionsAnswerTopicViewResponse>>>> GetAll()
        {
            try
            {
                var questionsAnswerTopicViews = await _questionsAnswerTopicViewService.GetAllAsync();
                return Ok(new ApiResponse<IEnumerable<QuestionsAnswerTopicViewResponse>>(1, "Lấy danh sách thành công", questionsAnswerTopicViews));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy danh sách", ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<QuestionsAnswerTopicViewResponse>>> GetById(int id)
        {
            try
            {
                var questionsAnswerTopicView = await _questionsAnswerTopicViewService.GetByIdAsync(id);
                if (questionsAnswerTopicView == null)
                {
                    return NotFound(new ApiResponse<QuestionsAnswerTopicViewResponse>(0, "Không tìm thấy bản ghi"));
                }
                return Ok(new ApiResponse<QuestionsAnswerTopicViewResponse>(1, "Lấy bản ghi thành công", questionsAnswerTopicView));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy bản ghi", ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<QuestionsAnswerTopicViewResponse>>> Create(QuestionsAnswerTopicViewRequest request)
        {
            try
            {
                var questionsAnswerTopicView = await _questionsAnswerTopicViewService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = questionsAnswerTopicView.Id }, new ApiResponse<QuestionsAnswerTopicViewResponse>(1, "Tạo bản ghi thành công", questionsAnswerTopicView));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<List<ValidationError>>(400, "Validation failed.", ex.Errors));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi tạo bản ghi", ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<QuestionsAnswerTopicViewResponse>>> Update(int id, QuestionsAnswerTopicViewRequest request)
        {
            try
            {
                var questionsAnswerTopicView = await _questionsAnswerTopicViewService.UpdateAsync(id, request);
                if (questionsAnswerTopicView == null)
                {
                    return NotFound(new ApiResponse<QuestionsAnswerTopicViewResponse>(0, "Không tìm thấy bản ghi"));
                }
                return Ok(new ApiResponse<QuestionsAnswerTopicViewResponse>(1, "Cập nhật bản ghi thành công", questionsAnswerTopicView));
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
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi cập nhật bản ghi", ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<QuestionsAnswerTopicViewResponse>>> Delete(int id)
        {
            try
            {
                var questionsAnswerTopicView = await _questionsAnswerTopicViewService.DeleteAsync(id);
                if (questionsAnswerTopicView == null)
                {
                    return NotFound(new ApiResponse<QuestionsAnswerTopicViewResponse>(0, "Không tìm thấy bản ghi"));
                }
                return Ok(new ApiResponse<QuestionsAnswerTopicViewResponse>(1, "Xóa bản ghi thành công", questionsAnswerTopicView));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi xóa bản ghi", ex.Message));
            }
        }
    }
}