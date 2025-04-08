using MailKit.Net.Smtp;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Project_LMS.Data;
using Project_LMS.Hubs;
using Project_LMS.Models;

public class TestExamNotificationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TestExamNotificationService> _logger;
    private readonly IConfiguration _config;
    private readonly IHubContext<RealtimeHub> _hubContext;

    public TestExamNotificationService(
        IServiceProvider serviceProvider,
        ILogger<TestExamNotificationService> logger,
        IConfiguration config,
        IHubContext<RealtimeHub> hubContext)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _config = config;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // Gửi thông báo cho các bài thi ngày mai (12h đêm)
                    await SendMidnightNotificationsAsync(context, stoppingToken);

                    // Gửi thông báo trước giờ thi
                    await SendNearTestTimeNotificationsAsync(context, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi thông báo lịch thi");
            }

            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }

    private async Task SendMidnightNotificationsAsync(ApplicationDbContext context, CancellationToken stoppingToken)
    {
        var now = DateTime.Now;
        if (now.Hour == 0 && now.Minute <= 5)
        {
            var nextDay = now.Date.AddDays(1);
            var upcomingExams = await GetUpcomingExamsAsync(context, nextDay, stoppingToken);
            await SendNotificationsForExams(context, upcomingExams, "Thông báo lịch thi ngày mai", true);
        }
    }

    private async Task SendNearTestTimeNotificationsAsync(ApplicationDbContext context, CancellationToken stoppingToken)
    {
        var now = DateTime.Now;
        var nearFutureTime = now.AddHours(1);

        var upcomingExams = await context.TestExams
            .Where(te => te.IsDelete == false
                         && te.StartDate.HasValue
                         && te.StartDate.Value > now
                         && te.StartDate.Value <= nearFutureTime)
            .Include(te => te.ClassTestExams)
            .ThenInclude(cte => cte.Class)
            .ThenInclude(c => c.ClassStudents)
            .ThenInclude(cs => cs.User)
            .Include(te => te.Subject)
            .ToListAsync(stoppingToken);

        await SendNotificationsForExams(context, upcomingExams, "Nhắc nhở: Sắp đến giờ thi", false);
    }

    private async Task<List<TestExam>> GetUpcomingExamsAsync(
        ApplicationDbContext context,
        DateTime targetDate,
        CancellationToken stoppingToken)
    {
        return await context.TestExams
            .Where(te => te.IsDelete == false
                         && te.StartDate.HasValue
                         && te.StartDate.Value.Date == targetDate)
            .Include(te => te.ClassTestExams)
            .ThenInclude(cte => cte.Class)
            .ThenInclude(c => c.ClassStudents)
            .ThenInclude(cs => cs.User)
            .Include(te => te.Subject)
            .ToListAsync(stoppingToken);
    }

    private async Task SendNotificationsForExams(
        ApplicationDbContext context,
        List<TestExam> exams,
        string subject,
        bool isMidnightNotification)
    {
        foreach (var exam in exams)
        {
            foreach (var classTest in exam.ClassTestExams.Where(cte => !cte.IsDelete.Value))
            {
                var students = classTest.Class.ClassStudents
                    .Where(cs => cs.IsActive.Value && !cs.IsDelete.Value && cs.UserId.HasValue)
                    .Select(cs => cs.User)
                    .Where(u => u != null);

                foreach (var student in students)
                {
                    var emailBody = isMidnightNotification
                        ? CreateMidnightNotificationEmail(exam, classTest.Class.Name, student.FullName)
                        : CreateNearTestTimeEmail(exam, classTest.Class.Name, student.FullName);

                    try
                    {
                        // Thử gửi email trước
                        if (!string.IsNullOrEmpty(student.Email))
                        {
                            await SendEmailAsync(student.Email, subject, emailBody);
                            _logger.LogInformation(
                                $"Đã gửi email thông báo cho học sinh {student.FullName} - {student.Email}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Lỗi gửi email cho {student.Email}");
                    }

                    // Gửi thông báo đẩy qua SignalR
                    try
                    {
                        var notificationContent = isMidnightNotification
                            ? CreateMidnightNotificationContent(exam, classTest.Class.Name, student.FullName)
                            : CreateNearTestTimeContent(exam, classTest.Class.Name, student.FullName);

                        await _hubContext.Clients.User(student.Id.ToString())
                            .SendAsync("ReceiveNotification", new
                            {
                                Subject = subject,
                                Content = notificationContent,
                                CreateAt = DateTime.UtcNow.ToString(),
                                Type = "System"
                            });

                        _logger.LogInformation(
                            $"Đã gửi thông báo đẩy cho học sinh {student.FullName} - ID: {student.Id}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Lỗi gửi thông báo đẩy cho học sinh ID: {student.Id}");
                    }
                }
            }
        }
    }

    private string CreateMidnightNotificationEmail(TestExam exam, string className, string studentName)
    {
        return $@"
                    <p>Xin chào {studentName},</p>
                    <p>Lớp {className} có lịch thi môn {exam.Subject?.SubjectName} vào ngày mai ({exam.StartDate?.ToString("dd/MM/yyyy HH:mm")}).</p>
                    <p>Thời gian làm bài: {exam.Duration} phút</p>
                    <p>Vui lòng chuẩn bị và tham gia đúng giờ.</p>
                    <p>Trân trọng,</p>
                    <p>Đội ngũ hỗ trợ</p>";
    }

    private string CreateNearTestTimeEmail(TestExam exam, string className, string studentName)
    {
        return $@"
                    <p>Xin chào {studentName},</p>
                    <p><strong>Nhắc nhở:</strong> Còn 1 tiếng nữa là đến giờ thi!</p>
                    <p>Lớp {className} - Môn {exam.Subject?.SubjectName}</p>
                    <p>Thời gian bắt đầu: {exam.StartDate?.ToString("HH:mm")}</p>
                    <p>Thời gian làm bài: {exam.Duration} phút</p>
                    <p>Vui lòng chuẩn bị và tham gia đúng giờ.</p>
                    <p>Trân trọng,</p>
                    <p>Đội ngũ hỗ trợ</p>";
    }

    private string CreateMidnightNotificationContent(TestExam exam, string className, string studentName)
    {
        return $"Lớp {className} có lịch thi môn {exam.Subject?.SubjectName} vào ngày mai ({exam.StartDate?.ToString("dd/MM/yyyy HH:mm")}). Thời gian làm bài: {exam.Duration} phút.";
    }

    private string CreateNearTestTimeContent(TestExam exam, string className, string studentName)
    {
        return $"Nhắc nhở: Còn 1 tiếng nữa là đến giờ thi! Lớp {className} - Môn {exam.Subject?.SubjectName}. Thời gian bắt đầu: {exam.StartDate?.ToString("HH:mm")}. Thời gian làm bài: {exam.Duration} phút.";
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var emailSender = _config["EmailSettings:SenderEmail"];
        var emailPassword = _config["EmailSettings:Password"] ?? _config["EmailSettings:SenderPassword"];
        var smtpServer = _config["EmailSettings:SmtpServer"];
        var smtpPortStr = _config["EmailSettings:Port"] ?? _config["EmailSettings:SmtpPort"];

        if (!int.TryParse(smtpPortStr, out int smtpPort))
        {
            throw new InvalidOperationException("Port SMTP không hợp lệ.");
        }

        using var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Support", emailSender));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(emailSender, emailPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}