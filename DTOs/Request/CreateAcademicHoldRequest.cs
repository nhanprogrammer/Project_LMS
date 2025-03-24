using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request;

public class CreateAcademicHoldRequest
{
    //public int Id { get; set; }

    public string? HoldDuration { get; set; }
    [Required(ErrorMessage = "User là bắt buộc.")]
    [Range(1, int.MaxValue, ErrorMessage = "UserId phải lớn hơn 0.")]
    public int? UserId { get; set; }

    [Required(ErrorMessage = "Class là bắt buộc.")]
    [Range(1, int.MaxValue, ErrorMessage = "ClassId phải lớn hơn 0.")]
    public int ClassId { get; set; }

    [Required(ErrorMessage = "HoldDate là bắt buộc.")]
    public DateTimeOffset HoldDate { get; set; }
    public string? Reason { get; set; }
    public string? FileName { get; set; }
    //public DateTimeOffset CreateAt { get; set; }
    [JsonIgnore]
    public int? UserCreate { get; set; }
}