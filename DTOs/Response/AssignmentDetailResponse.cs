namespace Project_LMS.DTOs.Response;

public class AssignmentDetailResponse
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public int QuestionId { get; set; }
    public int AnswerId { get; set; }
    public bool IsCorrect { get; set; }
    public bool IsDelete { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
    public int? UserCreate { get; set; }
    public int? UserUpdate { get; set; }
}
