namespace Project_LMS.DTOs.Response;

public class LessonResponse
{
    public int Id { get; set; }
    public int? TeachingAssignmentId { get; set; }
    public int? UserId { get; set; }
    public string? ClassLessonCode { get; set; }
    public string? Description { get; set; }
    public string? PaswordLeassons { get; set; }
    public string? Topic { get; set; }
    public string? Duration { get; set; }
    public string? LessonLink { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsResearchable { get; set; }
    public bool? IsAutoStart { get; set; }
    public bool? IsSave { get; set; }
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }

}