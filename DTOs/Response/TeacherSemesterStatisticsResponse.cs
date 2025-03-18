namespace Project_LMS.DTOs.Response
{
    public class TeacherSemesterStatisticsResponse
    {
        public string SemesterName { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public List<ClassTeachingDetail> ClassTeachingDetails { get; set; } = new();
    }

    public class ClassTeachingDetail
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ClassStatus { get; set; } = string.Empty; // Active, Inactive, etc.
    }
}