namespace Project_LMS.DTOs.Response
{
    public class ClassFutureResponse
    {
        public int Id { get; set; }
        public string? ClassCode { get; set; }
        public string SubjectName { get; set; }
        public DateTime? StartDate { get; set; }
        public string? TeacherName { get; set; }
        public int? StatusClass { get; set; }
    }
}