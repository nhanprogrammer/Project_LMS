using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces;

namespace Project_LMS.Services;

public class TestExamNotificationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TestExamNotificationService> _logger;

    public TestExamNotificationService(
        IServiceProvider serviceProvider,
        ILogger<TestExamNotificationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
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
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationsService>();

                    // Tìm các bài thi diễn ra ngày mai
                    var nextDay = DateTime.Now.Date.AddDays(1);

                    var upcomingExams = await context.TestExams
                        .Where(te => te.IsDelete == false
                                     && te.IsExam == true
                                     && te.StartDate.HasValue
                                     && te.StartDate.Value.Date == nextDay)
                        .Include(te => te.ClassTestExams)
                        .ThenInclude(cte => cte.Class)
                        .Include(te => te.Subject)
                        .ToListAsync(stoppingToken);

                    foreach (var exam in upcomingExams)
                    {
                        foreach (var classTest in exam.ClassTestExams.Where(cte => !cte.IsDelete.Value))
                        {
                            var subject = "Thông báo lịch thi ngày mai";
                            var content =
                                $"Lớp {classTest.Class.Name} có lịch thi môn {exam.Subject?.SubjectName} vào ngày mai ({exam.StartDate?.ToString("dd/MM/yyyy")}). " +
                                $"Thời gian: {exam.Duration} phút.";

                            await notificationService.SendClassNotificationAsync(
                                0,
                                classTest.ClassId.Value,
                                subject,
                                content,
                                true
                            );

                            _logger.LogInformation($"Đã gửi thông báo lịch thi cho lớp {classTest.Class.Name}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi thông báo lịch thi");
            }

            // Tính thời gian chạy tiếp theo vào 00:00
            var now = DateTime.Now;
            var nextRun = now.Date.AddDays(1); // 00:00 ngày hôm sau
            var delay = nextRun - now;

            await Task.Delay(delay, stoppingToken);
        }
    }
}