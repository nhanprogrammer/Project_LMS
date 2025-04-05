namespace Project_LMS.DTOs.Request;

public class SaveEssayRequest
{
    public int? TestExamId { get; set; }
    public string? SubmissionFile { get; set; }
    
    public List<string> AttachedFiles { get; set; } 
    
}