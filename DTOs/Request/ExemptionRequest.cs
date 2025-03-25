using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class ExemptionRequest
    {
        public string? UserCode { get; set; }  // Khóa ngoại tham chiếu Users

        public string? ExemptedSubject { get; set; }  // Môn học được miễn

        public string? Expression { get; set; }  // Lý do miễn
    }
}
