using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class SystemSettingRequest
    {
        [Required(ErrorMessage = "CaptchaEnabled không được để trống.")]
        public bool? CaptchaEnabled { get; set; }

        [Required(ErrorMessage = "CurrentTheme không được để trống.")]
        [StringLength(50, ErrorMessage = "CurrentTheme không được vượt quá 50 ký tự.")]
        public string? CurrentTheme { get; set; }

        [Required(ErrorMessage = "Language không được để trống.")]
        [StringLength(30, ErrorMessage = "Language không được vượt quá 30 ký tự.")]
        public string? Language { get; set; }
    }
}