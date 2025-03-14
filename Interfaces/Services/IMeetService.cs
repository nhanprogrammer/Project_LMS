// Services/IMeetService.cs
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public interface IMeetService
    {
        Task<ActionResult<ClassOnline>> CreateRoomAsync(CreateRoomRequest request);
        Task<bool> SendQuestionAnswer(int classOnlineId, int userId, string content);
    }
}