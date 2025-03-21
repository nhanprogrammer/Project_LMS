namespace Project_LMS.DTOs.Request
{
    public class TeachingAssignmentRequest
    {
        //public int? UserId { get; set; }
        public int? ClassId { get; set; }
        public int? SubjectId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        //public string? Describe { get; set; }
    }
    public class TeachingAssignmentRequestCreate
    {
        public int? UserId { get; set; }
        public int? ClassId { get; set; }
        public int? SubjectId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        //public string? Describe { get; set; }
    }
}
