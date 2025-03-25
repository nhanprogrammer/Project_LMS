using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Project_LMS.Helpers;

namespace Project_LMS.DTOs.Request
{
    public class CreateSemesterRequest
    {
        [Required(ErrorMessage = "Tên học kỳ là bắt buộc")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly DateStart { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly DateEnd { get; set; }
    }
}