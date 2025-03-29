namespace Project_LMS.DTOs.Response;

public class TestExamScheduleResponse
{
    public string SubjectAndDuration { get; set; }
    public string? ClassName { get; set; }
    public string? DepartmentName { get; set; }
    public DateTimeOffset? StartDate { get; set; }
  
}

public class TestExamScheduleDetailResponse
{
    public int TestExamId { get; set; }
    public string TeacherName { get; set; }
    public string Duration { get; set; }
    public string TestExamType { get; set; }
    public string? Form { get; set; }
    
}


