// using Microsoft.AspNetCore.SignalR;
// using Microsoft.EntityFrameworkCore;
// using Project_LMS.Data;
// using Project_LMS.DTOs.Response;
// using Project_LMS.Models;

// namespace Project_LMS.Services
// {
//     public class MeetHubService : Hub
//     {
//         private readonly ApplicationDbContext _context;

//         public MeetHubService(ApplicationDbContext context)
//         {
//             _context = context;
//         }

//         public override async Task OnConnectedAsync()
//         {
//             var roomId = Context.GetHttpContext()?.Request.Query["roomId"].ToString();
//             if (!string.IsNullOrEmpty(roomId))
//             {
//                 await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
//                 await UpdateUserList(roomId);
//             }
//             await base.OnConnectedAsync();
//         }

//         public override async Task OnDisconnectedAsync(Exception? exception)
//         {
//             var roomId = Context.GetHttpContext()?.Request.Query["roomId"].ToString();
//             if (!string.IsNullOrEmpty(roomId))
//             {
//                 await UpdateUserList(roomId);
//             }
//             await base.OnDisconnectedAsync(exception);
//         }

//         private async Task UpdateUserList(string roomId)
//         {
//             var classOnline = await _context.ClassOnlines.FirstOrDefaultAsync(c => c.ClassCode == roomId);
//             if (classOnline == null) return;

//             var users = await _context.ClassStudentOnlines
//                 .Where(cs => cs.ClassId == classOnline.Id && cs.IsDelete != true)
//                 .Select(cs => new UserInRoomResponse
//                 {
//                     UserId = cs.UserId ?? 0,
//                     FullName = cs.users.FullName,
//                     Image = cs.users.Image,
//                     RoleName = cs.users.roles.name,
//                     IsMuted = cs.IsMuted ?? false,
//                     IsCamera = cs.IsCamera ?? false,
//                     IsAdmin = cs.IsAdmin ?? false,
//                     JoinTime = cs.JoinTime
//                 }).ToListAsync();

//             await Clients.Group(roomId).SendAsync("UserListUpdated", users);
//         }

//         public async Task SendMessage(int classOnlineId, int userId, string content, string roomId)
//         {
//             var message = new ChatMessage
//             {
//                 ClassOnlineId = classOnlineId,
//                 UserId = userId,
//                 MessageContent = content,
//                 createAt = DateTime.Now
//             };

//             _context.ChatMessages.Add(message);
//             await _context.SaveChangesAsync();

//             var response = new ChatResponse(
//                 message.Id,
//                 roomId,
//                 message.users?.Image ?? "",
//                 message.users?.FullName ?? "Unknown",
//                 message.users?.roles?.name ?? "User",
//                 message.createAt,
//                 message.MessageContent
//             );

//             await Clients.Group(roomId).SendAsync("ReceiveMessage", response);
//         }
//     }
// }
