namespace Project_LMS.DTOs.Request
{
    public class ClassRequest
    {
        public int AcademicYearId { get; set; } = 0;
        public int DepartmentId { get; set; } = 0;
        public int PageNumber { get; set; } = 1; 
        public int PageSize { get; set; } = 10; 
        public string? Key { get; set; }
    }
}
   