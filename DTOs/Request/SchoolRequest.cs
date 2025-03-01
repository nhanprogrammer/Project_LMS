using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class SchoolRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã trường là bắt buộc")]
        public string SchoolCode { get; set; } = null!;

        [Required(ErrorMessage = "Tên trường là bắt buộc")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Hiệu trưởng là bắt buộc")]
        public string Principal { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại hiệu trưởng là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PrincipalPhone { get; set; } = null!;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Website là bắt buộc")]
        [Url(ErrorMessage = "URL không hợp lệ")]
        public string Website { get; set; } = null!;

        [Required(ErrorMessage = "Tỉnh/Thành phố là bắt buộc")]
        public string Province { get; set; } = null!;

        [Required(ErrorMessage = "Loại trường là bắt buộc")]
        public string SchoolType { get; set; } = null!;

        [Required(ErrorMessage = "Mô hình giáo dục là bắt buộc")]
        public string EducationModel { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Ngày thành lập là bắt buộc")]
        public DateTime? EstablishmentDate { get; set; }
    }
}