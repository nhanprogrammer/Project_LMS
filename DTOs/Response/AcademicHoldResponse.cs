using System.Collections;

namespace Project_LMS.DTOs.Response;

public class AcademicHoldResponse
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string? UserCode { get; set; }
    public string? FullName { get; set; }
    public DateTimeOffset? BirthDate { get; set; }
    public string? Gender { get; set; }
    public string? ClassName { get; set; }

    public DateTimeOffset? HoldDate { get; set; }
    public int? HoldDuration { get; set; }
    public string? Reason { get; set; }
    //public string? FileName { get; set; }
    //public DateTimeOffset? CreateAt { get; set; }
    //public int? UserCreate { get; set; }
    //public bool IsDelete { get; set; }


}
