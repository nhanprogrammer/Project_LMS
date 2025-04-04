namespace Project_LMS.DTOs.Response
{
    public class SubjectResponse
    {
        public int Id { get; set; }
        public int SubjectTypeId { get; set; }
        public int? SubjectGroupId { get; set; } 
        public bool? IsStatus { get; set; }
        public string? SubjectCode { get; set; }
        public string? SubjectName { get; set; }
        public string? Description { get; set; }
        public int? Semester1PeriodCount { get; set; }
        public int? Semester2PeriodCount { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}