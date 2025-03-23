using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class MeetCloseRequest
    {
        [Required(ErrorMessage = "RoomId là bắt buộc.")]
        public string? RoomId { get; set; }
    }
}