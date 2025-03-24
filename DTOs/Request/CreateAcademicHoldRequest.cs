using System.Text.Json.Serialization;

namespace Project_LMS.DTOs.Request;

public class CreateAcademicHoldRequest
{
    //public int Id { get; set; }
    public int ClassId { get; set; }
    public int? UserId { get; set; }
    public DateTimeOffset HoldDate { get; set; }
    public string? HoldDuration { get; set; }
    public string? Reason { get; set; }
    public string? FileName { get; set; }
    //public DateTimeOffset CreateAt { get; set; }
    [JsonIgnore]
    public int? UserCreate { get; set; }
}