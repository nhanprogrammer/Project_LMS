namespace Project_LMS.DTOs.Request
{
    public class CreateSubjectGroupRequest
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime? CreateAt { get; set; }
        public int? UserCreate { get; set; }
        
        public List<int> SubjectIds { get; set; } = new List<int>();
    }
}