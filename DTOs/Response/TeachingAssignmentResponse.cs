namespace Project_LMS.DTOs.Response
{
    public class TeachingAssignmentResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? ClassId { get; set; }
        public int? SubjectId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
