using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "RefreshToken không được để trống.")]
        public string? RefreshToken { get; set; }
    }
}
