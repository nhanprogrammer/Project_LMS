namespace Project_LMS.DTOs.Response
{
    public class SemesterResponse
    {
        public int Id { get; set; }
        public int AcademicYearId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool? IsDelete { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
    }
}