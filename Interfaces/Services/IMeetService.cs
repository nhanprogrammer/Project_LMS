// Services/IMeetService.cs
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public interface IMeetService
    {
        Task<ClassOnlineResponse?> JoinOrCreateClassOnlineForTeacher(CreateRoomRequest request);
        Task<ClassOnlineResponse?> JoinClassOnlineForStudent(CreateRoomRequest request);
        Task<bool> CloseRoom(CreateRoomRequest request);
        Task<bool> KickUserFromRoom(int lessonId, int userId);
    }
}