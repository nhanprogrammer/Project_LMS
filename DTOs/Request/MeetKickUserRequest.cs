using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class MeetKickUserRequest
    {
        [Required(ErrorMessage = "RoomId là bắt buộc.")]
        public string? RoomId { get; set; }

        [Required(ErrorMessage = "UserId là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "UserId phải lớn hơn 0.")]
        public int UserId { get; set; }
    }
}