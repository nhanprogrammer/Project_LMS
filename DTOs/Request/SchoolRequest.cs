using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class SchoolRequest
    {
        [Required(ErrorMessage = "Mã trường là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mã trường không được vượt quá 50 ký tự")]
        public string SchoolCode { get; set; } = null!;

        [Required(ErrorMessage = "Tên trường là bắt buộc")]
        [StringLength(255, ErrorMessage = "Tên trường không được vượt quá 255 ký tự")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Hiệu trưởng là bắt buộc")]
        [StringLength(255, ErrorMessage = "Tên hiệu trưởng không được vượt quá 255 ký tự")]
        public string Principal { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại hiệu trưởng là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại hiệu trưởng không hợp lệ")]
        [StringLength(255, ErrorMessage = "Số điện thoại hiệu trưởng không được vượt quá 255 ký tự")]
        public string PrincipalPhone { get; set; } = null!;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Website là bắt buộc")]
        [Url(ErrorMessage = "URL không hợp lệ")]
        [StringLength(255, ErrorMessage = "Website không được vượt quá 255 ký tự")]
        public string Website { get; set; } = null!;

        [Required(ErrorMessage = "Tỉnh/Thành phố là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tỉnh/Thành phố không được vượt quá 50 ký tự")]
        public string Province { get; set; } = null!;

        [Required(ErrorMessage = "Quận/Huyện là bắt buộc")]
        [StringLength(50, ErrorMessage = "Quận/Huyện không được vượt quá 50 ký tự")]
        public string District { get; set; } = null!;

        [Required(ErrorMessage = "Xã/Phường là bắt buộc")]
        [StringLength(50, ErrorMessage = "Xã/Phường không được vượt quá 50 ký tự")]
        public string Ward { get; set; } = null!;

        [StringLength(50, ErrorMessage = "Số fax không được vượt quá 50 ký tự")]
        public string? Fax { get; set; }
        public bool? IsJuniorHigh { get; set; }
        public bool? IsHighSchool { get; set; }

        [Required(ErrorMessage = "Mô hình giáo dục là bắt buộc")]
        [StringLength(255, ErrorMessage = "Mô hình giáo dục không được vượt quá 255 ký tự")]
        public string EducationModel { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(10, ErrorMessage = "Số điện thoại không được vượt quá 10 ký tự")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Ngày thành lập là bắt buộc")]
        public DateTime? EstablishmentDate { get; set; }

        [Required(ErrorMessage = "Trụ sở chính là bắt buộc")]
        [StringLength(255, ErrorMessage = "Trụ sở chính không được vượt quá 255 ký tự")]
        public string? HeadOffice { get; set; }
        public string? Image { get; set; }
    }
}