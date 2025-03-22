using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

public interface IEmailService
{
    Task SendOtpAsync(string email, string otp);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendOtpAsync(string email, string otp)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_config["EmailSettings:SenderName"], _config["EmailSettings:SenderEmail"]));
        message.To.Add(new MailboxAddress(email, email));
        message.Subject = "Your OTP Code";
        message.Body = new TextPart("html")
        {
            Text = $"<h3>Your OTP Code: <strong>{otp}</strong></h3><p>This OTP is valid for 5 minutes.</p>"
        };

        var smtpServer = _config["EmailSettings:SmtpServer"];
        var port = int.Parse(_config["EmailSettings:Port"]);
        var senderEmail = _config["EmailSettings:SenderEmail"];
        var password = _config["EmailSettings:Password"];
        var useAuth = bool.Parse(_config["EmailSettings:UseAuthentication"]);

        using (var smtp = new SmtpClient())
        {
            // Bỏ qua kiểm tra chứng chỉ SSL
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

            try
            {
                await smtp.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);

                if (useAuth)
                {
                    await smtp.AuthenticateAsync(senderEmail, password);
                }

                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send OTP email to {email}. Error: {ex.Message}", ex);
            }
        }
    }
}
