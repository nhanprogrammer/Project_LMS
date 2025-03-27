using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class SchoolTransferRequest
    {

        [Required(ErrorMessage = "Mã học sinh là bắt buộc")]
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Mã chi nhánh trường là bắt buộc")]
        public int? SchoolBranchesId { get; set; }

        [Required(ErrorMessage = "Ngày chuyển trường là bắt buộc")]
        public DateTime? TransferDate { get; set; }

        [Required(ErrorMessage = "Học kỳ là bắt buộc")]
        public string Semester { get; set; } = null!;

        [Required(ErrorMessage = "Lý do chuyển trường là bắt buộc")]
        public string Reason { get; set; } = null!;
    }
}
