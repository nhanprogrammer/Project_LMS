using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.Interfaces.Services;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Project_LMS.Controllers
{
    [Authorize(Policy = "TEACHER")]
    [Authorize(Policy = "STUDENT")]
    [ApiController]
    [Route("api/[controller]")]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;
        private readonly IAuthService _authService;

        public TopicController(ITopicService topicService, IAuthService authenticationService)
        {
            _topicService = topicService;
            _authService = authenticationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTopics([FromQuery] int teachingAssignmentId)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                // Kiểm tra teachingAssignmentId
                if (teachingAssignmentId <= 0)
                {
                    return BadRequest(new { Status = 1, Message = "TeachingAssignmentId phải lớn hơn 0!" });
                }

                var result = await _topicService.GetAllTopicsAsync(user.Id, teachingAssignmentId);
                if (result.Status == 1)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Status = 1, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 1, Message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTopicById(int id, [FromQuery] int teachingAssignmentId)
        {
            try
            {
                // Lấy userId từ token
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                var userId = user.Id;

                // Kiểm tra id và teachingAssignmentId
                if (id <= 0)
                {
                    return BadRequest(new { Status = 1, Message = "Id phải lớn hơn 0!" });
                }

                if (teachingAssignmentId <= 0)
                {
                    return BadRequest(new { Status = 1, Message = "TeachingAssignmentId phải lớn hơn 0!" });
                }

                var result = await _topicService.GetTopicByIdAsync(userId, teachingAssignmentId, id);
                if (result.Status == 1)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Status = 1, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 1, Message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        [Authorize(Policy = "TEACHER")]
        [HttpPost]
        public async Task<IActionResult> CreateTopic([FromBody] CreateTopicRequest request)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                // Lấy userId từ token và so sánh với request.UserId
                var userId = user.Id;
                request.UserId = userId;
                // Kiểm tra TeachingAssignmentId
                if (!request.TopicId.HasValue && request.TeachingAssignmentId <= 0)
                {
                    return BadRequest(new { Status = 1, Message = "TeachingAssignmentId là bắt buộc khi tạo topic!" });
                }

                var result = await _topicService.CreateTopicAsync(request);
                if (result.Status == 1)
                {
                    return BadRequest(result);
                }

                // Trả về 201 Created theo chuẩn REST
                return CreatedAtAction(nameof(GetTopicById),
                    new { id = result.Data?.Id, teachingAssignmentId = request.TeachingAssignmentId }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Status = 1, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 1, Message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        [Authorize(Policy = "TEACHER")]
        [HttpPut]
        public async Task<IActionResult> UpdateTopic([FromBody] UpdateTopicRequest request)
        {
            try
            {
                // Lấy userId từ token và so sánh với request.UserId
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                var userId = user.Id;
                Console.WriteLine("Update topic userId: " + userId);

                request.UserId = userId;

                // Kiểm tra TeachingAssignmentId
                if (!request.TopicId.HasValue && !request.TeachingAssignmentId.HasValue)
                {
                    return BadRequest(new
                        { Status = 1, Message = "TeachingAssignmentId là bắt buộc khi cập nhật topic!" });
                }

                var result = await _topicService.UpdateTopicAsync(request);
                if (result.Status == 1)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Status = 1, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 1, Message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(int id, [FromQuery] int teachingAssignmentId)
        {
            try
            {
                // Lấy userId từ token
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                var userId = user.Id;

                // Kiểm tra id và teachingAssignmentId
                if (id <= 0)
                {
                    return BadRequest(new { Status = 1, Message = "Id phải lớn hơn 0!" });
                }

                if (teachingAssignmentId <= 0)
                {
                    return BadRequest(new { Status = 1, Message = "TeachingAssignmentId phải lớn hơn 0!" });
                }

                var result = await _topicService.DeleteTopicAsync(userId, teachingAssignmentId, id);
                if (result.Status == 1)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Status = 1, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 1, Message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchTopics([FromQuery] int teachingAssignmentId, [FromQuery] string? keyword)
        {
            try
            {
                // Lấy userId từ token
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                var userId = user.Id;

                // Kiểm tra teachingAssignmentId
                if (teachingAssignmentId <= 0)
                {
                    return BadRequest(new { Status = 1, Message = "TeachingAssignmentId phải lớn hơn 0!" });
                }

                var result = await _topicService.SearchTopicsAsync(userId, teachingAssignmentId, keyword);
                if (result.Status == 1)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Status = 1, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 1, Message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }
    }
}