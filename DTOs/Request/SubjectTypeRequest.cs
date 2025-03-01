namespace Project_LMS.DTOs.Request
{
    public class SubjectTypeRequest
    {
        public string Name { get; set; } = string.Empty;
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
    }
}
