using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class SubjectRequest
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Loại môn học không được bỏ trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Loại môn học phải lớn hơn 0")]
        public int SubjectTypeId { get; set; }
        [Required(ErrorMessage = "Phân công giảng dạy không được bỏ trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Phân công giảng dạy phải lớn hơn 0")]
        public int TeachingAssignmentId { get; set; }
        [Required(ErrorMessage = "Trạng thái không được bỏ trống")]
        public bool? IsStatus { get; set; }
        [Required(ErrorMessage = "Mã môn học không được bỏ trống")]
        public string? SubjectCode { get; set; }
        [Required(ErrorMessage = "Tên môn học không được bỏ trống")]
        public string? SubjectName { get; set; }
        [Required(ErrorMessage = "Mô tả không được bỏ trống")]
        public string? Description { get; set; }
        [Range(0, 100, ErrorMessage = "Số tiết học kỳ 1 phải từ 0-100")]
        public int? Semester1PeriodCount { get; set; }
        [Range(0, 100, ErrorMessage = "Số tiết học kỳ 2 phải từ 0-100")]
        public int? Semester2PeriodCount { get; set; }
    }
}
