using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [Authorize(Policy = "STUDENT,TEACHER")]
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsAnswersController : ControllerBase
    {
        private readonly IQuestionsAnswersService _questionsAnswersService;
        private readonly IAuthService _authService;

        public QuestionsAnswersController(IQuestionsAnswersService questionsAnswersService, IAuthService authService)
        {
            _questionsAnswersService = questionsAnswersService;
            _authService = authService;
        }

        /// <summary>
        /// Lấy danh sách tất cả câu hỏi với phân trang
        /// </summary>
        /// <param name="request">Thông tin phân trang (page, pageSize)</param>
        /// <returns>Danh sách câu hỏi phân trang</returns>
        /// <response code="200">Trả về danh sách câu hỏi thành công</response>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequest request)
        {
            var result = await _questionsAnswersService.GetAllAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một câu hỏi theo ID, bao gồm lượt xem
        /// </summary>
        /// <param name="id">ID của câu hỏi cần lấy</param>
        /// <param name="userId">ID của người dùng (nếu có), để kiểm tra lượt xem</param>
        /// <returns>Thông tin chi tiết của câu hỏi</returns>
        /// <response code="200">Trả về thông tin câu hỏi thành công</response>
        /// <response code="404">Không tìm thấy câu hỏi</response>
        /// <response code="400">Yêu cầu không hợp lệ</response>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

            var result = await _questionsAnswersService.GetByIdWithViewAsync(id, user.Id);
            if (result.Status == 1)
            {
                return NotFound(result);
            }
            else if (result.Status == 2)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Tạo một câu hỏi hoặc câu trả lời mới
        /// </summary>
        /// <param name="request">Thông tin câu hỏi/câu trả lời cần tạo</param>
        /// <returns>Kết quả tạo câu hỏi/câu trả lời</returns>
        /// <response code="200">Tạo câu hỏi/câu trả lời thành công</response>
        /// <response code="400">Yêu cầu không hợp lệ hoặc có lỗi khi tạo</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuestionsAnswerRequest request)
        {


            var user = await _authService.GetUserAsync();
            System.Console.WriteLine($"USER INFO: ID={user?.Id}, Name={user?.FullName}, Role={user?.RoleId}");
            System.Console.WriteLine($"REQUEST INFO: TeachingAssignmentId={request.TeachingAssignmentId}, LessonId={request.LessonId}");

            if (user == null)
                return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

            request.UserId = user.Id;
            var result = await _questionsAnswersService.AddAsync(request);

            // Kiểm tra status từ ApiResponse để quyết định loại response
            if (result.Status == 0)
            {
                return Ok(result); // 200 OK khi thành công
            }
            else
            {
                System.Console.WriteLine($"ERROR: {result.Message}");
                return BadRequest(result); // 400 Bad Request khi có lỗi
            }
        }

        /// <summary>
        /// Cập nhật thông tin của một câu hỏi hoặc câu trả lời
        /// </summary>
        /// <param name="request">Thông tin cập nhật của câu hỏi/câu trả lời</param>
        /// <returns>Kết quả cập nhật</returns>
        /// <response code="200">Cập nhật thành công</response>
        /// <response code="400">Yêu cầu không hợp lệ hoặc có lỗi khi cập nhật</response>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateQuestionsAnswerRequest request)
        {
            var user = await _authService.GetUserAsync();
            System.Console.WriteLine($"UPDATE USER INFO: ID={user?.Id}, Name={user?.FullName}, Role={user?.RoleId}");
            System.Console.WriteLine($"UPDATE REQUEST INFO: Id={request.Id}, Message={request.Message}");

            if (user == null)
                return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

            request.UserUpdate = user.Id;
            var result = await _questionsAnswersService.UpdateAsync(request);
            
            if (result.Status == 0)
            {
                return Ok(result); // 200 OK khi thành công
            }
            else
            {
                System.Console.WriteLine($"UPDATE ERROR: {result.Message}");
                return BadRequest(result); // 400 Bad Request khi có lỗi
            }
        }

        /// <summary>
        /// Xóa một câu hỏi hoặc câu trả lời theo ID
        /// </summary>
        /// <param name="id">ID của câu hỏi/câu trả lời cần xóa</param>
        /// <returns>Kết quả xóa</returns>
        /// <response code="200">Xóa thành công</response>
        /// <response code="404">Không tìm thấy câu hỏi/câu trả lời để xóa</response>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

            var result = await _questionsAnswersService.DeleteAsync(id, user.Id);
            if (!result.Data)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách tất cả câu hỏi thuộc một topic theo TopicId
        /// </summary>
        /// <param name="topicId">ID của topic cần lấy danh sách câu hỏi</param>
        /// <returns>Danh sách câu hỏi thuộc topic</returns>
        /// <response code="200">Trả về danh sách câu hỏi thành công</response>
        /// <response code="404">Không tìm thấy topic hoặc không có câu hỏi nào</response>
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

        /// <summary>
        /// Lấy danh sách thành viên của lớp học theo TeachingAssignmentId, bao gồm thống kê
        /// </summary>
        /// <param name="teachingAssignmentId">ID của phân công giảng dạy</param>
        /// <returns>Danh sách thành viên lớp học cùng với thống kê</returns>
        /// <response code="200">Trả về danh sách thành viên thành công</response>
        /// <response code="404">Không tìm thấy phân công giảng dạy hoặc không có thành viên</response>
        /// <response code="400">Yêu cầu không hợp lệ</response>
        /// <response code="500">Lỗi hệ thống</response>
        [HttpGet("class-members/{teachingAssignmentId:int}")]
        public async Task<IActionResult> GetClassMembersByTeachingAssignment(int teachingAssignmentId,
            string? searchTerm = null)
        {
            try
            {
                var result =
                    await _questionsAnswersService.GetClassMembersByTeachingAssignmentAsync(teachingAssignmentId,
                        searchTerm);

                if (result.Status == 1)
                {
                    // Nếu không tìm thấy thành viên hoặc teachingAssignmentId không hợp lệ
                    if (result.Message.Contains("Không tìm thấy") || result.Message.Contains("không tồn tại"))
                    {
                        return NotFound(result);
                    }

                    // Nếu lỗi khác (ví dụ: lỗi hệ thống)
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<ClassMembersWithStatsResponse>(
                    1, $"Lỗi hệ thống: {ex.Message}", null);
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Lấy danh sách sinh viên thuộc một phân công giảng dạy
        /// </summary>
        /// <param name="teachingAssignmentId">ID của phân công giảng dạy</param>
        /// <returns>Danh sách sinh viên</returns>
        /// <response code="200">Trả về danh sách sinh viên thành công</response>
        /// <response code="404">Không tìm thấy phân công giảng dạy hoặc không có sinh viên</response>
        /// <response code="400">Yêu cầu không hợp lệ</response>
        [HttpGet("teaching-assignment/{teachingAssignmentId:int}/students")]
        public async Task<IActionResult> GetStudentsByTeachingAssignment(int teachingAssignmentId)
        {
            var result = await _questionsAnswersService.GetStudentsByTeachingAssignmentAsync(teachingAssignmentId);
            if (result.Status == 1)
            {
                return NotFound(result);
            }
            else if (result.Status == 2)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách câu hỏi hoặc topic theo tab (Tất cả câu hỏi, Đã trả lời, Gần đến hạn, Topics)
        /// </summary>
        /// <param name="tab">Tên tab: all, answered, near-deadline, topics</param>
        /// <param name="teachingAssignmentId">ID của phân công giảng dạy</param>
        /// <param name="userId">ID của người dùng</param>
        /// <returns>Danh sách câu hỏi hoặc topic theo tab</returns>
        /// <response code="200">Trả về danh sách câu hỏi/topic thành công</response>
        /// <response code="400">Yêu cầu không hợp lệ hoặc có lỗi</response>
        [HttpGet("by-tab")]
        public async Task<IActionResult> GetQuestionsAnswersByTab(
            [FromQuery] string tab,
            [FromQuery] int teachingAssignmentId,
            [FromQuery] int lessonId
            )
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

            // Gọi service để xử lý
            var result =
                await _questionsAnswersService.GetQuestionsAnswersByTabAsync(user.Id, teachingAssignmentId, tab, lessonId);

            if (result.Status == 0)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // Thêm endpoint này để debug
        [AllowAnonymous]
        [HttpGet("debug-permissions")]
        public async Task<IActionResult> DebugPermissions()
        {
            try
            {
                var user = await _authService.GetUserAsync();

                if (user == null)
                    return Ok(new { Message = "Người dùng chưa đăng nhập hoặc token không hợp lệ" });

                // Lấy claims từ token
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var claims = identity?.Claims.Select(c => new { Type = c.Type, Value = c.Value }).ToList();

                // Kiểm tra role của user
                var roles = HttpContext.User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                return Ok(new
                {
                    UserId = user.Id,
                    UserName = user.FullName,
                    UserRole = user.RoleId,
                    Roles = roles,
                    Claims = claims,
                    IsAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }
    }
}