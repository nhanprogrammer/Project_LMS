namespace Project_LMS.DTOs.Request;

public class NotificationResponse
{
    public int? Id { get; set; }
    public int? SenderId { get; set; }
    public int? ClassId { get; set; }
    public int? TestExamId { get; set; }
    public string? Subject { get; set; }
    public string? Content { get; set; }
    public bool? IsType { get; set; }
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public bool? IsDelete { get; set; }
    public int? UserCreate { get; set; }
    public int? UserUpdate { get; set; }
}