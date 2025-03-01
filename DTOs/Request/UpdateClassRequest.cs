namespace Project_LMS.DTOs.Request
{
    public class UpdateClassRequest
    {
        public int Id { get; set; }
        public int AcademicYearId { get; set; }
        public int DepartmentId { get; set; }
        public int ClassTypeId { get; set; }
        public string? Description { get; set; }
        public string ClassCode { get; set; }
        public DateTime? UpdateAt { get; set; } = DateTime.UtcNow;
        public int UserUpdate { get; set; }
    }
}
