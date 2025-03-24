namespace Project_LMS.DTOs.Request
{

    using System.ComponentModel.DataAnnotations;

    public class AuthLoginRequest
    {
        [Required(ErrorMessage = "Username không được bỏ trống")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        public string? Password { get; set; }
    }
    public class AuthUserLoginResponse
    {
        public string UserName { get; set; }
        public string Fullname { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public List<string> Permission { get; set; }

        public AuthUserLoginResponse(string userName, string fullname, string token, string role, List<string> permission)
        {
            UserName = userName;
            Fullname = fullname;
            Token = token;
            Role = role;
            Permission = permission;
        }
    }


    public class AuthForgotPasswordRequest
    {
        [Required(ErrorMessage = "Username không được bỏ trống")]
         [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }
    }

    public class AuthResetPasswordRequest
    {
        [Required(ErrorMessage = "UserName không được bỏ trống")]
       
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Mã xác thực không được bỏ trống")]
        public string? VerificationCode { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được bỏ trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được bỏ trống")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string? ConfirmPassword { get; set; }
    }

}