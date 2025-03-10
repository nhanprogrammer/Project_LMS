namespace Project_LMS.DTOs.Response;

public class ClassDetailResponse
{
    public int Id { get; set; }
    public String? AcademicYearName { get; set; } // niên khóa
    public String? DepartmentName { get; set; } // khoa khối
    public string? ClassCode { get; set; }
    public string? ClassName { get; set; }
    public string? HomeroomTeacher { get; set; }
    public String? StudentCount { get; set; }
    public String? ClassType { get; set; }
    public String? SubjectCount { get; set; }
    public string? Description { get; set; }
    public List<ClassDetailStudentResponse> ClassDetailStudentResponse { get; set; } = new();
    public List<ClassDetailSubjectResponse> ClassDetailSubjectResponse { get; set; } = new();

}