namespace Project_LMS.DTOs.Request;

public class NotificationsReceiverRequest
{
    public int? Id { get; set; }
    public int? NotificationId { get; set; }
    public int? ReceiverId { get; set; }
    public bool? Status { get; set; }
    public bool? IsDelete { get; set; }
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public int? UserCreate { get; set; }
    public int? UserUpdate { get; set; }
}