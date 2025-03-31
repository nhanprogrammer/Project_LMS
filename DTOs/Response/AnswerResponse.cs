namespace Project_LMS.DTOs.Response;

public class AnswerResponse
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string Answer { get; set; }
    public bool IsCorrect { get; set; }
    public bool IsDelete { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
    public int? UserCreate { get; set; }
    public int? UserUpdate { get; set; }
  
}
