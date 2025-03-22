// Controllers/MeetController.cs
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;
using Project_LMS.Services;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetController : ControllerBase
    {
        private readonly IMeetService _meetService;

        public MeetController(IMeetService meetService)
        {
            _meetService = meetService;
        }

        // API tạo phòng học
        [HttpPost("create-room")]
        public async Task<ActionResult<ClassOnline>> CreateRoom([FromBody] CreateRoomRequest request)
        {
            return await _meetService.CreateRoomAsync(request);
        }

        // API lấy danh sách người dùng trong phòng
        // [HttpGet("users/{roomId}")]
        // public async Task<ActionResult<IEnumerable<UserInRoomResponse>>> GetUsersInRoom(string roomId)
        // {
        //     return await _meetService.GetUsersInRoomAsync(roomId);
        // }
    }
}