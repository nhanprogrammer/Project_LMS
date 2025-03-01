namespace Project_LMS.DTOs.Request;

public class UpdateLessonRequest
{
    public int ClassId { get; set; }
    public int TeacherId { get; set; }
    public string ClassLessonCode { get; set; } = null!;
    public string? Description { get; set; }
    public string? Topic { get; set; }
    public string? Duration { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Password { get; set; }
    public bool? IsResearchable { get; set; }
    public bool? IsAutoStart { get; set; }
    public bool? IsSave { get; set; }
    public DateTime? UpdateAt { get; set; }
    public bool? IsDelete { get; set; }
    public int? UserUpdate { get; set; }
}