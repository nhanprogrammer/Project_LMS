using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class SchoolBranchRequest
    {

        public int? Id { get; set; }
        [Required(ErrorMessage = "Mã trường là bắt buộc")]
        public int? SchoolId { get; set; }

        [Required(ErrorMessage = "Tên chi nhánh là bắt buộc")]
        public string BranchName { get; set; } = null!;

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(10, ErrorMessage = "Số điện thoại không được vượt quá 10 ký tự")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại trường là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại trường không hợp lệ")]
        [StringLength(10, ErrorMessage = "Số điện thoại trường không được vượt quá 10 ký tự")]
        public string? SchoolPhone { get; set; } = null!;

        [Required(ErrorMessage = "Người quản lý là bắt buộc")]
        public string Manager { get; set; } = null!;

        [Required(ErrorMessage = "Mã tỉnh/thành phố là bắt buộc")]
        public int? ProvinceId { get; set; }

        [Required(ErrorMessage = "Mã quận/huyện là bắt buộc")]
        public int? DistrictId { get; set; }

        [Required(ErrorMessage = "Mã phường/xã là bắt buộc")]
        public int? WardId { get; set; }
        public string? Image { get; set; }
    }
}