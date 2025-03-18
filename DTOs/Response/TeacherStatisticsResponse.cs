namespace Project_LMS.DTOs.Response
{
    public class TeacherStatisticsResponse
    {
        public int TotalClasses { get; set; }
        public int TotalOnlineClasses { get; set; }
        public int TotalUngradedAssignments { get; set; }
        public int TotalQuestionsReceived { get; set; }
    }
}