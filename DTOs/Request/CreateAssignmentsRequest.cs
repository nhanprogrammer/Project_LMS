namespace Project_LMS.DTOs.Request;

public class CreateAssignmentRequest
{
    public int TestExamId { get; set; }
    public int StudentId { get; set; }
    public DateTime Submission { get; set; }
    public int? TotalScore { get; set; }
    public string? SubmissionFile { get; set; }
    public string? Comment { get; set; }
    public DateTime? CreateAt { get; set; }
    public int? UserCreate { get; set; }
}
