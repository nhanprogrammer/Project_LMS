namespace Project_LMS.DTOs.Request;

public class UpdateAcademicHoldRequest
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public int ClassId { get; set; }
    public DateTime HoldDate { get; set; }
    public string? HoldDuration { get; set; }
    public string? Reason { get; set; }
    public string? FileName { get; set; }

}