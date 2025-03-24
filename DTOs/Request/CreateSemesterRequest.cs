using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class CreateSemesterRequest
    {
        [Required(ErrorMessage = "Tên học kỳ là bắt buộc")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime? DateStart { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        public DateTime? DateEnd { get; set; }
    }
}