namespace Project_LMS.DTOs.Request
{
    public class TopicRequest
    {
        public int? TeachingAssignmentId { get; set; }
        public int? UserId { get; set; }
        public int? TopicId { get; set; }

        public string? Title { get; set; }
        public string? FileName { get; set; }
        public string? Description { get; set; }
        public DateTime? CloseAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
    }
}
