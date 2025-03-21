namespace Project_LMS.DTOs.Response
{
    public class StudentSemesterStatisticsResponse
    {
        public string SemesterName { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public List<StudentClassDetail> ClassDetails { get; set; } = new();
    }

    public class StudentClassDetail
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty; // Chu học/Đã học/Đã hoãn thành
        public ClassScheduleDetail? FirstSchedule { get; set; } // Buổi học đầu tiên
    }
}