namespace Project_LMS.DTOs.Request
{
    public class TranscriptRequest
    {
        public int? AcademicYearId { get; set; } //Năm học
        public int? DepartmentId { get; set; } //Khối

        public string UserCode { get; set; }
        public int? SemesterId { get; set; }
    }
}
