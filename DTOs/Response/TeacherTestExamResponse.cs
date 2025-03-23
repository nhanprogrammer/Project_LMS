namespace Project_LMS.DTOs.Response;

public class TeacherTestExamResponse
{
    public int Id   { get; set; }
    public string ClassName { get; set; }
    public string ContentTest { get; set; }
    public string SubjectName { get; set; }
    public string DateOfExam { get; set; }
    public string Duration { get; set; }
    public string Status { get; set; }
}