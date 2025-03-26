using Project_LMS.DTOs.Request;
using Project_LMS.Models;

namespace Project_LMS.Interfaces
{
    public interface IAuthService
    {
        Task<AuthUserLoginResponse> LoginAsync(string userName, string password);
        Task LogoutAsync(HttpContext context);
        Task SendVerificationCodeAsync(string email);
        Task ResetPasswordWithCodeAsync(string userName, string verificationCode, string newPassword, string confirmPassword);
        Task<User?> GetUserAsync();
        Task<string> HashPassword(string password);

    }

}