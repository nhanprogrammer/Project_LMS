using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Project_LMS.Interfaces.Responsitories;

namespace Project_LMS.Services;

public class NotificationQueueService : BackgroundService
{
    private readonly ILogger<NotificationQueueService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentQueue<NotificationQueueItem> _notificationQueue;
    private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

    public NotificationQueueService(ILogger<NotificationQueueService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _notificationQueue = new ConcurrentQueue<NotificationQueueItem>();
    }

    public void QueueNotification(NotificationQueueItem notification)
    {
        if (notification == null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        _notificationQueue.Enqueue(notification);
        _signal.Release(); // Thông báo thread xử lý rằng có thông báo mới
    }

    public void QueueNotificationToUsers(int? senderId, List<int> userIds, string subject, string content, bool type)
    {
        foreach (var userId in userIds)
        {
            QueueNotification(new NotificationQueueItem
            {
                SenderId = senderId,
                UserId = userId,
                Subject = subject,
                Content = content,
                Type = type
            });
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification Queue Service đang chạy.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Đợi thông báo mới (với timeout để kiểm tra stoppingToken)
                await _signal.WaitAsync(TimeSpan.FromSeconds(5), stoppingToken);

                // Kiểm tra và xử lý hàng đợi
                while (_notificationQueue.TryDequeue(out var notification))
                {
                    try
                    {
                        await ProcessNotificationAsync(notification);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi khi xử lý thông báo: {Error}", ex.Message);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Bỏ qua khi token bị cancel, sẽ thoát vòng lặp
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định trong NotificationQueueService: {Error}", ex.Message);
                
                // Đợi một chút trước khi thử lại để tránh CPU spike nếu có lỗi liên tục
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        _logger.LogInformation("Notification Queue Service đã dừng.");
    }

    private async Task ProcessNotificationAsync(NotificationQueueItem notification)
    {
        // Sử dụng scope để đảm bảo các service được dispose đúng cách
        using var scope = _serviceProvider.CreateScope();
        var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationsRepository>();

        _logger.LogInformation("Đang gửi thông báo cho userId: {UserId}, subject: {Subject}", 
            notification.UserId, notification.Subject);

        await notificationRepository.AddNotification(
            notification.SenderId,
            notification.UserId,
            notification.Subject,
            notification.Content,
            notification.Type
        );

        _logger.LogInformation("Đã gửi thông báo thành công cho userId: {UserId}", notification.UserId);
    }
}

public class NotificationQueueItem
{
    public int? SenderId { get; set; }
    public int UserId { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public bool Type { get; set; }
} 