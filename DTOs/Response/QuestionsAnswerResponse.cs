namespace Project_LMS.DTOs.Response
{
    public class QuestionsAnswerResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? Avatar { get; set; }
        public string? FullName { get; set; }
        public string? Message { get; set; }
        public string? FileName { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? RoleName { get; set; }
        public int ReplyCount { get; set; } // Số lượng câu trả lời
        public int Views { get; set; }
        public List<QuestionsAnswerResponse> Replies { get; set; } = new List<QuestionsAnswerResponse>(); // Danh sách câu trả lời
        
    }
}