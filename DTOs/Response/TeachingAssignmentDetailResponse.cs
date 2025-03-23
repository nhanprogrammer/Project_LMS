using Project_LMS.DTOs.Response;

public class TeachingAssignmentDetailResponse
{
    public int Id { get; set; }
    public string? ClassName { get; set; }
    public string? Description { get; set; }
    public string? Name { get; set; }
    public int TotalLessons { get; set; }
    public string? SubjectName { get; set; }
    public string? TeacherName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<ClassLessonResponse> Lessons { get; set; } = new List<ClassLessonResponse>();
}