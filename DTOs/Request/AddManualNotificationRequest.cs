namespace Project_LMS.DTOs.Request;

public class AddManualNotificationRequest
{
    public int SenderId { get; set; }
    public int UserId { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
}