using System.ComponentModel.DataAnnotations;
namespace Project_LMS.DTOs.Request
{
    public class QuestionRequest
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "TestExamId is required")]
        public int? TestExamId { get; set; }
        [Required(ErrorMessage = "Question text is required")]
        public string Question { get; set; } = null!;
        [Required(ErrorMessage = "Mark is required")]
        [Range(0, 10, ErrorMessage = "Mark must be between 0 and 10")]
        public int? Mark { get; set; }
    }
}