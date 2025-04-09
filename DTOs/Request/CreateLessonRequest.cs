using System.ComponentModel.DataAnnotations;
using Project_LMS.Helpers;

namespace Project_LMS.DTOs.Request;

public class CreateLessonRequest
{
    public int Id { get; set; }
    [Required(ErrorMessage = "TeachingAssignmentId không được bỏ trống")]
    [Range(1, int.MaxValue, ErrorMessage = "TeachingAssignmentId phải lớn hơn 0")]
    public int? TeachingAssignmentId { get; set; }
    [Required(ErrorMessage = "Mã người trợ giảng không được bỏ trống")]
    [Range(1, int.MaxValue, ErrorMessage = "Mã người trợ giảng phải lớn hơn 0")]
    public int? UserId { get; set; }
    public string? ClassLessonCode { get; set; }
    [Required(ErrorMessage = "Mô tả không được bỏ trống")]
    public string? Description { get; set; }
    [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
    public string? PaswordLeassons { get; set; }
    [Required(ErrorMessage = "Chủ đề không được bỏ trống")]
    public string? Topic { get; set; }
    [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Thời lượng phải là số")]
    [Range(45, int.MaxValue, ErrorMessage = "Thời lượng phải lớn hơn hoặc bằng 45 phút")]
    public string? Duration { get; set; }
    [Url(ErrorMessage = "Link buổi học không hợp lệ")]
    public string? LessonLink { get; set; }
    [Required(ErrorMessage = "Ngày bắt đầu không được bỏ trống")]
    [DataType(DataType.DateTime)]
    [CustomValidation(typeof(ProjectDateTimeValidator), "ValidateFutureDate", ErrorMessage = "Ngày bắt đầu phải lớn hơn ngày hiện tại")]
    public DateTime? StartDate { get; set; }
    [Required(ErrorMessage = "Ngày kết thúc không được bỏ trống")]
    [DataType(DataType.DateTime)]
    [CustomValidation(typeof(ProjectDateTimeValidator), "ValidateEndDate", ErrorMessage = "Ngày kết thúc phải lớn hơn ngày bắt đầu")]
    public DateTime? EndDate { get; set; }
    public bool? IsResearchable { get; set; }
    public bool? IsAutoStart { get; set; }
    public bool? IsSave { get; set; }

}
