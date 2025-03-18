namespace Project_LMS.DTOs.Response
{
    public class TeacherPerformanceReport
    {
        public int TotalClasses { get; set; }
        public int TotalExcellentStudents { get; set; }
        public int TotalGoodStudents { get; set; }
        public int TotalAverageStudents { get; set; }
        public int TotalWeakStudents { get; set; }
    }
}