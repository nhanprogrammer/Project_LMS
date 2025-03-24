namespace Project_LMS.DTOs.Response;

public class AcademicYearResponse
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool? IsInherit { get; set; }
    public int? AcademicParent { get; set; }
    public List<SemesterResponse> Semesters { get; set; }
}
