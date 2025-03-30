namespace Project_LMS.DTOs.Request
{
    public class TranscriptRequest
    {
        public int DepartmentId { get; set; } //Khối
        public int studentId { get; set; }
        public int SemesterId { get; set; }
    }
    public class TranscriptTeacherRequest
    {
        public int ClassId { get; set; }
        public int SemesterId { get; set; }
        public int SubjectId { get; set; }
        public string? searchItem { get; set; }
    }
}
