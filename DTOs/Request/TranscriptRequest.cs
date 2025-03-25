namespace Project_LMS.DTOs.Request
{
    public class TranscriptRequest
    {
        public int AcademicYearId { get; set; } //Năm học
        public int ClassId { get; set; } //Lớp học
        public int SubjectId { get; set; } //Môn học
        public int DepartmentId { get; set; } //Khối
    }
}
