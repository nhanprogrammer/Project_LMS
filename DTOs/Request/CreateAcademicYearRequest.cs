using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Project_LMS.Helpers;

namespace Project_LMS.DTOs.Request
{
    public class CreateAcademicYearRequest
    {
        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly EndDate { get; set; }

        public bool? IsInherit { get; set; }
        public int? AcademicParent { get; set; }
        public List<CreateSemesterRequest> Semesters { get; set; }
    }
}
