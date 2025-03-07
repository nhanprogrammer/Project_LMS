namespace Project_LMS.DTOs.Response;

public class ClassDetailStudentResponse
{
    public int Id { get; set; }
    public string? StudentCode { get; set; }
    public string? StudentName { get; set; }
    public String? AcademicYear { get; set; } // niên khóa
    public string? AdmissionDate { get; set; }
    public string? StudentStatus { get; set; }
    public int StudentStatusId { get; set; }

}