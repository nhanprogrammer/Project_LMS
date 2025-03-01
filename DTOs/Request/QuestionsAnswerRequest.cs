namespace Project_LMS.DTOs.Request;

public class QuestionsAnswerRequest
{
    public int? Id { get; set; }
    public int? TeachingAssignmentId { get; set; }
    public int? UserId { get; set; }
    public int? QuestionsAnswerId { get; set; }
    public string? Message { get; set; }
    public string? FileName { get; set; }
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public bool? IsDelete { get; set; }
    public int? UserCreate { get; set; }
    public int? UserUpdate { get; set; }
}