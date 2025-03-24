using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request;

public class UpdateAcademicYearRequest
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool? IsInherit { get; set; }
    public int? AcademicParent { get; set; }
    public List<UpdateSemesterRequest> Semesters { get; set; }
}
