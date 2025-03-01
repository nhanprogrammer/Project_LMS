namespace Project_LMS.DTOs.Response
{
    public class SubjectTypeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
