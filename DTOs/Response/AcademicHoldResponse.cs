namespace Project_LMS.DTOs.Response;

public class AcademicHoldResponse
{
    public string StudentCode { get; set; }
    public string StudentName { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
    public string ClassCode { get; set; }
    public DateTime HoldDate { get; set; }
    public string SemesterName { get; set; }
    public string Reason { get; set; }
}
