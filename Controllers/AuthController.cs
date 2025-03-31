using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
        {
            try
            {
                var userResponse = await _authService.LoginAsync(request.UserName, request.Password);

                // Lưu Access Token vào cookie (thời hạn ngắn,  1 ngày)
                Response.Cookies.Append("AccessToken", userResponse.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // Tắt tạm thời nếu frontend không có HTTPS
                    Expires = DateTime.Now.AddHours(24)
                });

                // Lưu Refresh Token vào cookie (thời hạn dài hơn,  6 tháng)
                Response.Cookies.Append("RefreshToken", userResponse.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    Expires = DateTime.Now.AddMonths(6)
                });

                return Ok(new ApiResponse<AuthUserLoginResponse>(0, "Đăng nhập thành công!", userResponse));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(1, ex.Message, null));
            }
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var newAccessToken = await _authService.RefreshAccessTokenAsync(request.RefreshToken);

                return Ok(new ApiResponse<string>(0, "Làm mới token thành công!", newAccessToken));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(2, "Lỗi hệ thống", null));
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _authService.LogoutAsync(HttpContext);
                return Ok(new ApiResponse<string>(0, "Đăng xuất thành công!", null));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(1, ex.Message, null));
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> SendVerificationCode([FromBody] AuthForgotPasswordRequest request)
        {
            try
            {
                await _authService.SendVerificationCodeAsync(request.UserName);
                return Ok(new ApiResponse<string>(0, "Mã xác thực đã được gửi!", null));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(1, "Đã xảy ra lỗi!", null));
            }
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] AuthResetPasswordRequest request)
        {
            try
            {
                await _authService.ResetPasswordWithCodeAsync(request.UserName, request.VerificationCode);
                return Ok(new ApiResponse<string>(0, "Mật khẩu đã được gửi. Bạn đã có thể đăng nhập với mật khẩu mới!", null));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(1, ex.Message, null));
            }
        }

        [Authorize(Policy = "DATA-MNG-VIEW")]
        [HttpGet("user-info")]
        public async Task<IActionResult> GetUserInfo()
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                var userInfo = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                };

                return Ok(new ApiResponse<object>(0, "Lấy thông tin người dùng thành công!", userInfo));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(1, "Lỗi hệ thống: " + ex.Message, null));
            }
        }
    }

}
