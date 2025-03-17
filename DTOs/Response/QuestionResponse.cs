namespace Project_LMS.DTOs.Response
{
    public class QuestionResponse
    {
        public int Id { get; set; }
        public int TestExamId { get; set; }
        public string QuestionText { get; set; } = null!;
        public int Mark { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}