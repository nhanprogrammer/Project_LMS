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
        public string ClassStatus { get; set; } = string.Empty;
        public ClassScheduleDetail? FirstSchedule { get; set; } // Chỉ lưu một buổi học (buổi đầu tiên)
    }

    public class ClassScheduleDetail
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string DayOfWeek => StartTime.ToString("dddd"); // Thứ (ví dụ: Thứ 2)
        public string TimeRange => $"{StartTime:HH:mm} - {EndTime:HH:mm}"; // Khoảng thời gian (ví dụ: 8:00 - 9:30)
        public string Date => StartTime.ToString("dd/MM/yyyy");
    }
}