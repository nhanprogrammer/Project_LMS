using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class MeetHubService : Hub
    {

        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public MeetHubService(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var roomId = httpContext?.Request.Query["roomId"].ToString();
            var accessToken = httpContext?.Request.Query["access_token"].ToString();
            if (string.IsNullOrEmpty(accessToken))
            {
                accessToken = httpContext?.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            }
            Console.WriteLine($"[OnConnectedAsync] RoomId: {roomId}, Token: {accessToken}");

            if (!string.IsNullOrEmpty(accessToken))
            {
                httpContext.Items["Token"] = accessToken;
            }

            var user = await _authService.GetUserAsync();
            Console.WriteLine($"[OnConnectedAsync] User: {user?.FullName ?? "null"}");

            if (string.IsNullOrEmpty(roomId) || user == null)
            {
                await Clients.Caller.SendAsync("Error", "roomId không hợp lệ hoặc không xác định được người dùng.");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var roomId = Context.GetHttpContext()?.Request.Query["roomId"].ToString();
            await base.OnDisconnectedAsync(exception);
        }

   public async Task GetListChat(string roomId)
        {
            Console.WriteLine($"[GetListChat] Đang lấy danh sách tin nhắn cho phòng: {roomId}");

            var classOnline = await _context.ClassOnlines
                .FirstOrDefaultAsync(c => c.ClassOnlineCode == roomId);

            if (classOnline == null)
            {
                Console.WriteLine($"[GetListChat] ❌ Không tìm thấy phòng: {roomId}");
                await Clients.Caller.SendAsync("Error", "Không tìm thấy phòng.");
                return;
            }

            var messages = await _context.ChatMessages
                .Where(m => m.ClassOnlineId == classOnline.Id)
                .OrderBy(m => m.CreateAt)
                .Select(m => new ChatResponse(
                    m.Id,
                    m.User.Image ?? "Unknown",
                    m.User.FullName ?? "Unknown",
                    m.User.Role.Name ?? "Unknown",
                    m.MessageContent
                )).ToListAsync();

            Console.WriteLine($"[GetListChat] ✅ Đã tìm thấy {messages.Count} tin nhắn trong phòng {roomId}");
            await Clients.Caller.SendAsync("ChatListUpdated", messages);
        }
        public async Task CreateMessage(string roomId, string content)
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
            {
                await Clients.Caller.SendAsync("Error", "Không xác định được người dùng.");
                return;
            }

            var classOnline = await _context.ClassOnlines.FirstOrDefaultAsync(c => c.ClassOnlineCode == roomId);
            if (classOnline == null)
            {
                await Clients.Caller.SendAsync("Error", "Không tìm thấy phòng.");
                return;
            }

            var message = new ChatMessage
            {
                ClassOnlineId = classOnline.Id,
                UserId = user.Id,
                MessageContent = content,
                CreateAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            // Sau khi lưu tin nhắn, gọi GetListChat cho tất cả mọi người trong phòng
            var messages = await _context.ChatMessages
                .Where(m => m.ClassOnlineId == classOnline.Id)
                .OrderBy(m => m.CreateAt)
                .Select(m => new ChatResponse(
                    m.Id,
                    m.User.Image ?? "Unknown",
                    m.User.FullName ?? "Unknown",
                    m.User.Role.Name ?? "Unknown",
                    m.MessageContent
                )).ToListAsync();

            // Gửi danh sách tin nhắn mới nhất cho toàn bộ nhóm
            await Clients.Group(roomId).SendAsync("ChatListUpdated", messages);
        }


        // Lấy danh sách user trong phòng (real-time)
        // private async Task UpdateUserList(string roomId)
        // {
        //     var classOnline = await _context.ClassOnlines
        //         .FirstOrDefaultAsync(c => c.ClassOnlineCode == roomId);

        //     if (classOnline == null) return;

        //     var users = await _context.ClassStudentOnlines
        //         .Where(cs => cs.ClassOnlineId == classOnline.Id && cs.IsDelete != true)
        //         .Select(cs => new UserInRoomResponse
        //         {
        //             UserId = cs.UserId ?? 0,
        //             FullName = cs.User.FullName ?? "Unknown",
        //             JoinTime = cs.JoinTime
        //         }).ToListAsync();

        //     await Clients.Group(roomId).SendAsync("UserListUpdated", users);
        // }


        // API để FE lấy danh sách user khi cần
        // public async Task GetListUsersConnected(string roomId)
        // {
        //     await UpdateUserList(roomId);
        // }

        // API để lấy danh sách tin nhắn của phòng chat



    }
}
