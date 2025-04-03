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
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Role { get; set; }
        public List<string> Permission { get; set; }

        public AuthUserLoginResponse(string userName, string fullname, string accessToken, string refreshToken, string role, List<string> permission)
        {
            UserName = userName;
            Fullname = fullname;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Role = role;
            Permission = permission;
        }
    }


    public class AuthForgotPasswordRequest
    {
       [Required(ErrorMessage = "UserName không được bỏ trống")]
        public string? UserName { get; set; }
    }

    public class AuthResetPasswordRequest
    {
        [Required(ErrorMessage = "UserName không được bỏ trống")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Mã xác thực không được bỏ trống")]
        public string? VerificationCode { get; set; }
    }
}