namespace Project_LMS.DTOs.Response
{
    public class QuestionsAnswerResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? Message { get; set; }
        public string? FileName { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? RoleName { get; set; }
    }
}