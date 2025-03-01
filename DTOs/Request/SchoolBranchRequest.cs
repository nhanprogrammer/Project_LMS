using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class SchoolBranchRequest
    {
        public int Id { get; set; }

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
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Người quản lý là bắt buộc")]
        public string Manager { get; set; } = null!;
    }
}