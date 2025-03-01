using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class QuestionsAnswerTopicViewRequest
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "QuestionsAnswerId is required")]
        public int? QuestionsAnswerId { get; set; }
        [Required(ErrorMessage = "UserId is required")]
        public int? UserId { get; set; }
        [Required(ErrorMessage = "TopicId is required")]
        public int? TopicId { get; set; }
    }
}