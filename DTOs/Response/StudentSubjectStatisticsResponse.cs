namespace Project_LMS.DTOs.Response
{
    public class StudentSubjectStatisticsResponse
    {
        public int TotalSubjects { get; set; }
        public int CompletedSubjects { get; set; }
        public int OngoingSubjects { get; set; }
    }
}