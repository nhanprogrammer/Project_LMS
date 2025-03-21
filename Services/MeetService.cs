// // Services/MeetService.cs
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Project_LMS.Data;
// using Project_LMS.DTOs.Request;
// using Project_LMS.DTOs.Response;
// using Project_LMS.Models;

// namespace Project_LMS.Services
// {
//     public class MeetService : IMeetService
//     {
//         private readonly ApplicationDbContext _context;

//         public MeetService(ApplicationDbContext context)
//         {
//             _context = context;
//         }

//        public async Task<ClassOnlineResponse?> CreateClassOnline(CreateRoomRequest  request)
//         {
//             var classOnline = new ClassOnline
//             {
//                 ClassCode = request.ClassCode,
//                 Title = request.Title,
//                 Description = request.Description,
//                 CreatedAt = DateTime.UtcNow
//             };

//             _context.ClassOnlines.Add(classOnline);
//             await _context.SaveChangesAsync();

//             return new ClassOnlineResponse
//             {
//                 Id = classOnline.Id,
//                 ClassCode = classOnline.ClassCode,
//                 Title = classOnline.Title,
//                 Description = classOnline.Description
//             };
//         }

//         public async Task<bool> SendQuestionAnswer(int classOnlineId, int userId, string content)
//         {
//             var questionAnswer = new QuestionAnswer
//             {
//                 ClassOnlineId = classOnlineId,
//                 UserId = userId,
//                 MessageContent = content,
//                 createAt = DateTime.Now
//             };

//             _context.QuestionAnswers.Add(questionAnswer);
//             await _context.SaveChangesAsync();
//             return true;
//         }

//     }
// }