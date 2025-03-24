using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{

    public class CreateRoomRequest
    {
        [Required(ErrorMessage = "LessonId là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "LessonId phải lớn hơn 0.")]
        public int? LessonId { get; set; }
    }
}