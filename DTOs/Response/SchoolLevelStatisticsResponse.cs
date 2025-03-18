namespace Project_LMS.DTOs.Response
{
    public class SchoolLevelStatisticsResponse
    {
        public int AcademicYearId { get; set; }
        public string SchoolLevel { get; set; }
        public List<GradeStatistics> GradeStatistics { get; set; }
    }

    public class GradeStatistics
    {
        public string? DepartmentCode { get; set; } 
        public int TotalStudents { get; set; }
    }
}