using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class CreateTopicRequest
    {
        public int? TeachingAssignmentId { get; set; }
        public int? UserId { get; set; }
        public int? TopicId { get; set; }
        [Required] public string? Title { get; set; }
        public string? FileName { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? CloseAt { get; set; }
    }
}