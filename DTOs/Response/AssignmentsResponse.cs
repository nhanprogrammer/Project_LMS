namespace Project_LMS.DTOs.Response
{
    public class AssignmentsResponse
    {
    public int Id { get; set; }
        public int TestExamId { get; set; }
        public int StudentId { get; set; }
        public DateTime Submission { get; set; }
        public int? TotalScore { get; set; }
        public string? SubmissionFile { get; set; }
        public string? Comment { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
    }
}
