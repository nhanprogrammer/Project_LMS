namespace Project_LMS.DTOs.Response 
{
    public class ClassStudentChangeResponse
    {
        public int UserId { get; set; }
        public string? UserCode { get; set; }
        public string? FullName { get; set; }
        public int ClassId { get; set; }
        public string? ClassName { get; set; }

    }
}