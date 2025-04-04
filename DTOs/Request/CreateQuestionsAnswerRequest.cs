namespace Project_LMS.DTOs.Request
{
    public class CreateQuestionsAnswerRequest
    {
        public int UserId { get; set; }
        public int ParentCommentId { get; set; }
        public string? Message { get; set; }
        public string? FileName { get; set; }
        public int TeachingAssignmentId { get; set; }
        public int? LessonId { get; set; }
    }
}


