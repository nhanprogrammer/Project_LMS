namespace Project_LMS.DTOs.Request;

public class CreateAssignmentDetailRequest
{
    public int AssignmentId { get; set; }
    public int QuestionId { get; set; }
    public int AnswerId { get; set; }
    public bool IsCorrect { get; set; }
    public DateTime? CreateAt { get; set; }
    public int? UserCreate { get; set; }
}
