namespace Project_LMS.DTOs.Request
{
    public class UpdateTopicRequest
    {
        public int Id { get; set; }
        public int? TeachingAssignmentId { get; set; }
        public int? UserId { get; set; }     
        public int? TopicId { get; set; }
        public string? Title { get; set; }
        public string? FileName { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? CloseAt { get; set; }
    }
}