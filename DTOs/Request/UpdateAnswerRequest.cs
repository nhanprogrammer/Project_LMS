namespace Project_LMS.DTOs.Request;

public class UpdateAnswerRequest
{
    public int QuestionId { get; set; }
    public string Answer { get; set; }
    public bool IsCorrect { get; set; }
    public bool? IsDelete { get; set; }
    public DateTime? UpdateAt { get; set; }
    public int? UserUpdate { get; set; }
}
