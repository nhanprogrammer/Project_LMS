using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class CreateAcademicYearRequest
    {
        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        public DateTime EndDate { get; set; }
        public bool? IsInherit { get; set; }
        public int? AcademicParent { get; set; }
        public List<CreateSemesterRequest> Semesters { get; set; }
    }
}
