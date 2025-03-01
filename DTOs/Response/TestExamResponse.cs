namespace Project_LMS.DTOs.Response
{
    public class TestExamResponse
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public int TestExamTypeId { get; set; }
        public string? Topic { get; set; }
        public string? Form { get; set; }
        public TimeOnly? Duration { get; set; }
        public bool? Classify { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Description { get; set; }
        public string? Attachment { get; set; }
        public string? SubmissionFormat { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
    }
}
