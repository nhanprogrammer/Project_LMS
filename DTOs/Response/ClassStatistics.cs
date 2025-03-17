namespace Project_LMS.DTOs.Response
{
    public class ClassStatistics
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
    }
}