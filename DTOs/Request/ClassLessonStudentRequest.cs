public class ClassLessonStudentRequest
{
    public int? UserId { get; set; }
    public string? Keyword { get; set; }
    public int Status { get; set; } = 0;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? AcademicYearId { get; set; }
    public DateTime? Date { get; set; }
    public int? DepartmentId { get; set; }
}