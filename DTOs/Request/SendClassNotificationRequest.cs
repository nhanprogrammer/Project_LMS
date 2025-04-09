namespace Project_LMS.DTOs.Request;

public class SendClassNotificationRequest
{
    public int ClassId { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
} 