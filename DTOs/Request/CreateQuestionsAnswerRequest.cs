namespace Project_LMS.DTOs.Request
{
    public class CreateQuestionsAnswerRequest
    {
        public int UserId { get; set; }
        public int TopicId { get; set; }
        public string? Message { get; set; }
        public string? FileName { get; set; }
        public int? QuestionsAnswerId { get; set; } // Nếu là trả lời của một câu hỏi/trả lời khác
    }
}