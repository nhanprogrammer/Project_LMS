namespace Project_LMS.DTOs.Request
{
    public class ClassSaveRequest
    {
        public int Id { get; set; }
        public int AcademicYearId { get; set; } // niên khóa
        public int DepartmentId { get; set; } // khoa khối
        public int ClassTypeId { get; set; }
        public int UserId { get; set; } // giáo viên chủ nhiệm
        public int StudentCount { get; set; }
        public string? Description { get; set; }
        public string? ClassName { get; set; }
        public List<int> Ids { get; set; } = new();
    }
}