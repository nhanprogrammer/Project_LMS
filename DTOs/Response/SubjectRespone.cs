namespace Project_LMS.DTOs.Response
{
    public class SubjectRespone
    {
        public int Id { get; set; }
        public string TypeSubjectName { get; set; } = string.Empty;
        public string SubjectGroupName { get; set; } = string.Empty;
        public bool? IsStatus { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
