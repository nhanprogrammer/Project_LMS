namespace Project_LMS.DTOs.Response
{
    public class TestExamResponse
    {
        public int Id { get; set; }
        public string? Semester { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public string? DepartmentName { get; set; }
        public string? SubjectName { get; set; }
        public string? Name { get; set; }
        public string? StatusExam { get; set; }
        public string? Examiner { get; set; }
        public string? ClassList { get; set; }
        public TimeOnly? Duration { get; set; } 
    }
}