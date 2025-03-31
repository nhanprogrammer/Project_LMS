namespace Project_LMS.DTOs.Response
{
    public class GradingDataResponse
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public int? ClassId { get; set; }
        public string ClassName { get; set; }
        public string Subject { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public TimeOnly Duration { get; set; }
        public string Form { get; set; }
        public string Description { get; set; }
        public string Attachment { get; set; }
        public bool IsExam { get; set; }
        public string? ProposalContent { get; set; }
        public List<StudentGradeResponse> StudentGrades { get; set; }
    }

    public class StudentGradeResponse
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public double? Score { get; set; }
        public string SubmissionStatus { get; set; }
        public string SubmissionFile { get; set; }
        public string Comment { get; set; }
        public string ClassStatus { get; set; }
        public DateTimeOffset? SubmissionDate { get; set; }
        public string SubmissionDuration { get; set; }
    }
}