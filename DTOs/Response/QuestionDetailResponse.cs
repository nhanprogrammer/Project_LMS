namespace Project_LMS.DTOs.Response;

public class QuestionDetailResponse
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string? FileName { get; set; }
    public int UserId { get; set; }
    public string UserFullName { get; set; }
    public string RoleName { get; set; }
    public DateTime CreateAt { get; set; }
    public List<AnswerInfoResponse> Answers { get; set; }
}

public class AnswerInfoResponse
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string? FileName { get; set; }
    public int UserId { get; set; }
    public string UserFullName { get; set; }
    public string RoleName { get; set; }
    public DateTime CreateAt { get; set; }
}