namespace Project_LMS.DTOs.Request;

    public class CreateClassRequest
    {
        public int AcademicYearId { get; set; }
        public int DepartmentId { get; set; }
        public int ClassTypeId { get; set; }
        public string? Description { get; set; }
        public string ClassCode { get; set; }
        public DateTime? CreateAt { get; set; } = DateTime.UtcNow;
        public int UserCreate { get; set; }
    }
