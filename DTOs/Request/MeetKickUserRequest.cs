using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class MeetKickUserRequest
    {
        [Required(ErrorMessage = "LessonId là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "LessonId phải lớn hơn 0.")]
        public int LessonId { get; set; }

        [Required(ErrorMessage = "UserId là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "UserId phải lớn hơn 0.")]
        public int UserId { get; set; }
    }
}