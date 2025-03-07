namespace Project_LMS.DTOs.Request
{
    public class SubjectRequest
    {
        public int SubjectTypeId { get; set; }
        public int TeachingAssignmentId { get; set; }
        public int SubjectGroupId { get; set; }
        public bool? IsStatus { get; set; }
        public string? SubjectCode { get; set; }
        public string? SubjectName { get; set; }
        public string? Description { get; set; }
        public int? Semester1PeriodCount { get; set; }
        public int? Semester2PeriodCount { get; set; }
    }
}
