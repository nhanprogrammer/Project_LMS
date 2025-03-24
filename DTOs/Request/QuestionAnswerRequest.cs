using System.ComponentModel.DataAnnotations;
namespace Project_LMS.DTOs.Request
{
    public class QuestionAnswerRequest
    {
        public string Content { get; set; } = null!;
        
        [Required(ErrorMessage = "RoomId là bắt buộc.")]
        public string? RoomId { get; set; }
    }
}