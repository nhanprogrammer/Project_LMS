namespace Project_LMS.DTOs.Response
{
    public class ClassResponse
    {
        public int Id { get; set; }
        public int AcademicYear { get; set; }
        public string DepartmentName { get; set; }
        public string ClassType { get; set; }
        public string? Description { get; set; }
        public string ClassCode { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
