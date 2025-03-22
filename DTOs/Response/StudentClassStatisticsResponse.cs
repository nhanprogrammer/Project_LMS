namespace Project_LMS.DTOs.Response
{
    public class StudentClassStatisticsResponse
    {
        public int TotalClasses { get; set; }
        public int CompletedClasses { get; set; }
        public int OngoingClasses { get; set; }
        public double AverageScore { get; set; }
    }
}