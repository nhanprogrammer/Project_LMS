namespace Project_LMS.DTOs.Request
{
    public class CreateAcademicYearRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool? IsInherit { get; set; }
        public int? AcademicParent { get; set; }
        public DateTime? CreateAt { get; set; }
        public int? UserCreate { get; set; }
    }
}
