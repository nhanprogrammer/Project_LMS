namespace Project_LMS.DTOs.Request;

public class NotificationResponse
{
    public int Id { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public string Type { get; set; } // "System" hoặc "User"
    public string SenderName { get; set; } // "Hệ thống" hoặc tên người gửi
    public DateTime CreateAt { get; set; }
    public bool IsRead { get; set; }
}