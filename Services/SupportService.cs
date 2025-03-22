using System.IdentityModel.Tokens.Jwt;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Project_LMS.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Project_LMS.Services
{
    public class SupportService : ISupportService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SupportService(IConfiguration configuration ,IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
             _httpContextAccessor = httpContextAccessor;
        }

        public async Task SendEmailAsync(string subject, string htmlBody, string token)
        {
            var senderEmail = GetSenderEmailFromToken(token);
            if (string.IsNullOrEmpty(senderEmail))
            {
                throw new UnauthorizedAccessException("Không tìm thấy email người gửi trong token.");
            }

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Người gửi", senderEmail));
            emailMessage.To.Add(new MailboxAddress("Người nhận", "nguyenminhkhanhst2018@gmail.com"));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html") { Text = htmlBody };

            // Cấu hình SMTP server
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var smtpUser = _configuration["EmailSettings:SmtpUser"];
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"];

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(smtpServer, smtpPort, false);
                await client.AuthenticateAsync(smtpUser, smtpPassword);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
        private string GetSenderEmailFromToken(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadToken(token) as JwtSecurityToken;
            var senderEmail = jwtToken?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            return senderEmail;
        }
    }
}