using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Services;
using System.Threading.Tasks;

namespace Project_LMS.Controllers
{
    [Route("api/meet")]
    [ApiController]
    public class MeetController : ControllerBase
    {
        private readonly IMeetService _meetService;

        public MeetController(IMeetService meetService)
        {
            _meetService = meetService ?? throw new ArgumentNullException(nameof(meetService));
        }

        // Tạo hoặc tham gia phòng cho giáo viên
        [HttpPost("join-room-teacher")]
        public async Task<ActionResult<ApiResponse<ClassOnlineResponse>>> CreateRoomForTeacher([FromBody] CreateRoomRequest request)
        {
            if (request?.LessonId == null)
            {
                return BadRequest(new ApiResponse<ClassOnlineResponse>(1, "LessonId không hợp lệ.", null));
            }

            try
            {
                var room = await _meetService.JoinOrCreateClassOnlineForTeacher(request);
                if (room == null)
                {
                    return StatusCode(500, new ApiResponse<ClassOnlineResponse>(1, "Không thể tạo hoặc tham gia phòng.", null));
                }
                return Ok(new ApiResponse<ClassOnlineResponse>(0, "Tạo hoặc tham gia phòng thành công!", room));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ClassOnlineResponse>(1, $"Lỗi khi tạo/tham gia phòng cho giáo viên: {ex.Message}", null));
            }
        }

        // Tham gia phòng cho học sinh
        [HttpPost("join-room-student")]
        public async Task<ActionResult<ApiResponse<ClassOnlineResponse>>> JoinRoomForStudent([FromBody] CreateRoomRequest request)
        {
            if (request?.LessonId == null)
            {
                return BadRequest(new ApiResponse<ClassOnlineResponse>(1, "LessonId không hợp lệ.", null));
            }

            try
            {
                var room = await _meetService.JoinClassOnlineForStudent(request);
                if (room == null)
                {
                    return StatusCode(500, new ApiResponse<ClassOnlineResponse>(1, "Không thể tham gia phòng.", null));
                }
                return Ok(new ApiResponse<ClassOnlineResponse>(0, "Tham gia phòng thành công!", room));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ClassOnlineResponse>(1, $"Lỗi khi tham gia phòng cho học sinh: {ex.Message}", null));
            }
        }

        // Đóng phòng
        [HttpPost("close-room")]
        public async Task<ActionResult<ApiResponse<string>>> CloseRoom([FromBody] CreateRoomRequest request)
        {
            if (request?.LessonId == null)
            {
                return BadRequest(new ApiResponse<string>(1, "LessonId không hợp lệ.", null));
            }

            try
            {
                var result = await _meetService.CloseRoom(request);
                if (!result)
                {
                    return StatusCode(500, new ApiResponse<string>(1, "Không thể đóng phòng.", null));
                }
                return Ok(new ApiResponse<string>(0, "Phòng đã được đóng thành công!", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Lỗi khi đóng phòng: {ex.Message}", null));
            }
        }

        // Kick user khỏi phòng
        [HttpPost("kick-user")]
        public async Task<ActionResult<ApiResponse<string>>> KickUserFromRoom([FromBody] MeetKickUserRequest request)
        {

            try
            {
                var result = await _meetService.KickUserFromRoom(request.LessonId, request.UserId);
                if (!result)
                {
                    return StatusCode(500, new ApiResponse<string>(1, "Không thể kick user.", null));
                }
                return Ok(new ApiResponse<string>(0, "User đã bị kick khỏi phòng thành công!", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Lỗi khi kick user: {ex.Message}", null));
            }
        }

    }
}