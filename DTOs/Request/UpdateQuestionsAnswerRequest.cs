namespace Project_LMS.DTOs.Request
{
    public class UpdateQuestionsAnswerRequest
    {
        public int Id { get; set; } // Định danh của câu hỏi/trả lời cần cập nhật
        public string? Message { get; set; }
        public string? FileName { get; set; }
    }
}