namespace Project_LMS.DTOs.Response
{
    public class TeachingAssignmentResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? ClassId { get; set; }
        public string? ClassName { get; set; }
        public int? SubjectId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        //public string? Describe {  get; set; }
        //public List<TopicResponse> Topics { get; set; } = new List<TopicResponse>();
    }
}
