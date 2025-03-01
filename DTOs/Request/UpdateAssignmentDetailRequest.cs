namespace Project_LMS.DTOs.Request;

public class UpdateAssignmentDetailRequest
{
    public int AssignmentId { get; set; }
    public int QuestionId { get; set; }
    public int AnswerId { get; set; }
    public bool IsCorrect { get; set; }
    public bool? IsDelete { get; set; }
    public DateTime? UpdateAt { get; set; }
    public int? UserUpdate { get; set; }
}
