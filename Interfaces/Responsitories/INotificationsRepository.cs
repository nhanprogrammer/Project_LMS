using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface INotificationsRepository
{
    Task AddNotification(int? senderId, int userId, string subject, string content, bool type);
    Task<IEnumerable<Notification>> GetNotificationsByUserId(int userId);
    Task<IEnumerable<Notification>> GetNotificationsByUserAndTeachingAssignment(int userId, int teachingAssignmentId);
    Task<IEnumerable<Notification>> GetAllNotificationsByUser(int userId);
    Task AddManualNotification(int senderId, int userId, string subject, string content);
    Task DeleteNotifications(int notificationId, int userId);
    Task SelectIsRead(int notificationId, int userId);
    Task AddNotificationToUsersAsync(List<int> userIds, string subject, string content);
}