using Project_LMS.DTOs.Request;
using Project_LMS.Models;

namespace Project_LMS.Interfaces
{
    public interface IAuthService
    {
        Task<AuthUserLoginResponse> LoginAsync(string userName, string password);
        Task<string?> RefreshAccessTokenAsync(string refreshToken);
        Task LogoutAsync(HttpContext context);
        Task SendVerificationCodeAsync(string userName);
        Task ResetPasswordWithCodeAsync(string userName, string verificationCode);
        Task<User?> GetUserAsync();
        Task<string> HashPassword(string password);

    }

}