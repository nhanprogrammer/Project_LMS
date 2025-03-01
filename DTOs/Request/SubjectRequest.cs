namespace Project_LMS.DTOs.Request
{
    public class SubjectRequest
    {
        public int TypeSubjectId { get; set; }
        public int SubjectGroupId { get; set; }
        public bool? IsStatus { get; set; }
        public string? Description { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
    }
}
