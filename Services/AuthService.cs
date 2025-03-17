
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.Interfaces;
using Project_LMS.Models;

namespace Project_LMS.Services
{

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IPermissionService _permissionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthService(ApplicationDbContext context, IConfiguration config, IPermissionService permissionService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _config = config;
            _permissionService = permissionService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthUserLoginResponse> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsDelete == false);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                throw new Exception("Email hoặc mật khẩu không đúng!");

            // user.PermissionChanged = false;
            // await _context.SaveChangesAsync();

            var token = await GenerateJwtToken(user);
            return new AuthUserLoginResponse(user.Email, user.FullName, token);
        }

        public async Task LogoutAsync(HttpContext context)
        {
            // Xóa cookie chứa token
            context.Response.Cookies.Delete("AuthToken");

            // Xóa token trong context để đảm bảo không còn lưu
            context.Items.Remove("Token");

            await Task.CompletedTask;
        }


        public async Task SendVerificationCodeAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsDelete == false);
            if (user == null) throw new KeyNotFoundException("Email không tồn tại!");

            var verificationCode = new Random().Next(100000, 999999).ToString();
            user.ResetCode = verificationCode;
            user.ResetCodeExpiry = DateTime.Now.AddMinutes(10);

            try
            {
                await _context.SaveChangesAsync();
                var emailBody = $@"
                    <p>Xin chào,</p>
                    <p>Mã xác thực của bạn là: <strong>{verificationCode}</strong></p>
                    <p>Mã này có hiệu lực trong <strong>10 phút</strong>. Vui lòng không chia sẻ mã này với bất kỳ ai.</p>
                    <p>Nếu bạn không yêu cầu đặt lại mật khẩu, hãy bỏ qua email này.</p>
                    <p>Trân trọng,</p>
                    <p><strong>Đội ngũ hỗ trợ</strong></p>";

                await SendEmailAsync(email, "Mã xác thực", emailBody);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Không thể gửi mã xác thực: {ex.Message}");
            }
        }


        public async Task ResetPasswordWithCodeAsync(string email, string verificationCode, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword) throw new Exception("Mật khẩu xác nhận không khớp!");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsDelete == false);
            if (user == null || user.ResetCode != verificationCode || user.ResetCodeExpiry < DateTime.UtcNow)
                throw new Exception("Mã xác thực không hợp lệ hoặc đã hết hạn!");

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ResetCode = null;
            user.ResetCodeExpiry = null;
            await _context.SaveChangesAsync();
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var permissions = await _permissionService.ListPermission(user.Id);
            if (permissions == null) permissions = new List<string>();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            foreach (var permission in permissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims, // Bây giờ claims là danh sách động
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var emailSender = _config["EmailSettings:SenderEmail"];
            var emailPassword = _config["EmailSettings:Password"] ?? _config["EmailSettings:SenderPassword"];
            var smtpServer = _config["EmailSettings:SmtpServer"];
            var smtpPortStr = _config["EmailSettings:Port"] ?? _config["EmailSettings:SmtpPort"];

            if (string.IsNullOrEmpty(emailSender) || string.IsNullOrEmpty(emailPassword) ||
                string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(smtpPortStr))
            {
                throw new InvalidOperationException("Cấu hình email không đầy đủ.");
            }

            if (!int.TryParse(smtpPortStr, out int smtpPort))
            {
                throw new InvalidOperationException("Port SMTP không hợp lệ.");
            }

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Support", emailSender));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(emailSender, emailPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (SmtpCommandException smtpEx)
            {
                throw new InvalidOperationException($"Lỗi SMTP: {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi gửi email: {ex.Message}");
            }
        }

        public async Task<string> HashPassword()
        {
            string password = "123456";
            return await Task.Run(() => BCrypt.Net.BCrypt.HashPassword(password));
        }

        public async Task<User?> GetUserAsync()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return null;

            var token = context.Items["Token"] as string;
            if (string.IsNullOrEmpty(token)) return null;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
                var email = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                if (email == null) return null;

                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsDelete == false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi đọc token: " + ex.Message);
                return null;
            }
        }
    }
}