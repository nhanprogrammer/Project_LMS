using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class SchoolTransferRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã học sinh là bắt buộc")]
        public int? StudentId { get; set; }

        [Required(ErrorMessage = "Mã chi nhánh trường là bắt buộc")]
        public int? SchoolBranchesId { get; set; }

        [Required(ErrorMessage = "Mã tỉnh/thành phố là bắt buộc")]
        public int? ProvinceId { get; set; }

        [Required(ErrorMessage = "Mã quận/huyện là bắt buộc")]
        public int? DistrictId { get; set; }

        [Required(ErrorMessage = "Mã phường/xã là bắt buộc")]
        public int? WardId { get; set; }

        [Required(ErrorMessage = "Ngày chuyển trường là bắt buộc")]
        public DateTime? TransferDate { get; set; }

        [Required(ErrorMessage = "Học kỳ là bắt buộc")]
        public string Semester { get; set; } = null!;

        [Required(ErrorMessage = "Lý do chuyển trường là bắt buộc")]
        public string Reason { get; set; } = null!;
    }
}
