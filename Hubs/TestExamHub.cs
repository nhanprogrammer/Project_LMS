using Microsoft.AspNetCore.SignalR;

namespace Project_LMS.Hubs;

public class TestExamHub : Hub
    
{
    public async Task SendStatusUpdate(int examId, string status)
    {
        await Clients.All.SendAsync("ReceiveStatusUpdate", examId, status);
    }
}