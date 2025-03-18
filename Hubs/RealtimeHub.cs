using Microsoft.AspNetCore.SignalR;

namespace Project_LMS.Hubs;

public class RealtimeHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // Bạn có thể thực hiện logging hoặc xử lý khi người dùng kết nối
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Xử lý khi người dùng ngắt kết nối, ví dụ: logging
        await base.OnDisconnectedAsync(exception);
    }
}