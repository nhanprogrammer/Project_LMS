// Services/IMeetService.cs
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public interface IMeetService
    {
        Task<ClassOnlineResponse?> GetOrCreateTeacherOnlineClass(CreateRoomRequest request);
        Task<ClassOnlineResponse?> JoinOnlineClass (CreateRoomRequest request);
        Task<bool> CloseRoom(MeetCloseRequest request);
        Task<bool> KickUserFromRoom(string RoomId, int userId);
        Task<bool> AddQuestionAnswer(QuestionAnswerRequest request);
    }
}