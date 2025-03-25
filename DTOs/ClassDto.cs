namespace Project_LMS.DTOs
{
    public class ClassDto
    {
        public int Id { get; set; }
        public int AcademicYearId { get; set; }
        public int DepartmentId { get; set; }
        public int ClassTypeId { get; set; }
        public string Description { get; set; }
        public string ClassCode { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool IsDelete { get; set; }
        public int UserCreate { get; set; }
    }
}
