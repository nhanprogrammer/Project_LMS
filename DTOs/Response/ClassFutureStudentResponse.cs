namespace Project_LMS.DTOs.Response
{
    public class ClassFutureStudentResponse
    {
        public int? TeachingAssignmentId { get; set; }
        public string? ClassCode { get; set; }
        public string? SubjectName { get; set; }
        public DateTime? StartDate { get; set; }
        public string? TeacherName { get; set; }
        public string? StatusClass { get; set; }
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        public double CompletionPercentage { get; set; }

    }
}