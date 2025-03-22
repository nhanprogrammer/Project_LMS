using Microsoft.AspNetCore.SignalR;

namespace Project_LMS.Hubs;

public class NameUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.Identity?.Name;
    }
}