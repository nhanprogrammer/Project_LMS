namespace Project_LMS.DTOs.Request;

public class CreateAnswerRequest
{
    public int QuestionId { get; set; }
    public string Answer { get; set; }
    public bool IsCorrect { get; set; }
    public DateTime? CreateAt { get; set; }
    public int? UserCreate { get; set; }
}
