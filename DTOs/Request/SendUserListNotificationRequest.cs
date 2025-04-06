namespace Project_LMS.DTOs.Request;

public class SendUserListNotificationRequest
{
    public List<int> UserIds { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public bool? Type { get; set; } 
}