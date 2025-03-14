using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class ClassSaveRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Niên khóa không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Niên khóa phải lớn hơn 0.")]
        public int AcademicYearId { get; set; }

        [Required(ErrorMessage = "Khoa khối không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Khoa khối phải lớn hơn 0.")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Loại lớp không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Loại lớp phải lớn hơn 0.")]
        public int ClassTypeId { get; set; }

        [Required(ErrorMessage = "Giáo viên chủ nhiệm không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Giáo viên chủ nhiệm phải lớn hơn 0.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Tên lớp không được để trống.")]
        [MinLength(2, ErrorMessage = "Tên lớp phải có ít nhất 2 ký tự.")]
        [MaxLength(100, ErrorMessage = "Tên lớp không được vượt quá 100 ký tự.")]
        public string? ClassName { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng học sinh phải là số không âm.")]
        public int StudentCount { get; set; }

        [MaxLength(500, ErrorMessage = "Mô tả không được dài quá 500 ký tự.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Danh sách môn học không được để trống.")]
        [MinLength(1, ErrorMessage = "Danh sách môn học phải có ít nhất một học sinh.")]
        public List<int> Ids { get; set; } = new List<int>();
    }
}
