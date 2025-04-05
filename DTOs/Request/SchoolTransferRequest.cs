using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_LMS.DTOs.Request
{
    public class SchoolTransferRequest
    {

        [Required(ErrorMessage = "Mã học sinh là bắt buộc")]
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Ngày chuyển trường là bắt buộc")]
        public DateTime? TransferDate { get; set; }

        [Required(ErrorMessage = "Học kỳ là bắt buộc")]
        public string Semester { get; set; } = null!;

        [Required(ErrorMessage = "Lý do chuyển trường là bắt buộc")]
        public string Reason { get; set; } = null!;       
        [Required(ErrorMessage = "Địa chỉ chuyển trường là bắt buộc")]
        public string Address { get; set; } = null!;       
        public string? FileName { get; set; } = null!;
        public string? TransferFrom { get; set; }
        public string? TransferTo { get; set; } 
        public string? ProvinceId { get; set; }
        public string? DistrictId { get; set; }
    }
}
