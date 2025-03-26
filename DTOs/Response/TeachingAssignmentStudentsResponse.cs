namespace Project_LMS.DTOs.Response;

public class TeachingAssignmentStudentsResponse
{
    public int TeachingAssignmentId { get; set; }
    public int TeacherId { get; set; }
    public string TeacherFullName { get; set; }
    public string ClassName { get; set; }
    public List<StudentInfoResponse> Students { get; set; }
}

public class StudentInfoResponse
{
    public int UserId { get; set; }
    public string FullName { get; set; }
    public string RoleName { get; set; }
}