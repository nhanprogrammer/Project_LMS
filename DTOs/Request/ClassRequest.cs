namespace Project_LMS.DTOs.Request
{
    public class ClassRequest
    {
        public int AcademicYearId { get; set; }
        public int DepartmentId { get; set; }
        public int PageNumber { get; set; } = 1; 
        public int PageSize { get; set; } = 10; 
    }
}
