using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;

namespace Project_LMS.Services;

public class NotificationsService : INotificationsService
{
    private readonly INotificationsRepository _notificationsRepository;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public NotificationsService(INotificationsRepository notificationsRepository, IMapper mapper,
        ApplicationDbContext context)
    {
        _notificationsRepository = notificationsRepository;
        _mapper = mapper;
        _context = context;
    }

    public async Task AddNotificationAsync(int? senderId, int userId, string subject, string content, bool type)
    {
        try
        {
            await _notificationsRepository.AddNotification(senderId, userId, subject, content, type);
        }
        catch (Exception ex)
        {
            throw new Exception($"Không thể thêm thông báo: {ex.Message}", ex);
        }
    }

    public async Task<ApiResponse<IEnumerable<NotificationResponse>>> GetNotificationsByUserIdAsync(int userId)
    {
        try
        {
            var notifications = await _notificationsRepository.GetNotificationsByUserId(userId);
            var notificationResponses = _mapper.Map<IEnumerable<NotificationResponse>>(notifications);

            foreach (var notification in notificationResponses)
            {
                if (notification.Type == "System")
                {
                    notification.SenderName = "Hệ thống";
                }
                else
                {
                    var sender =
                        await _context.Users.FindAsync(notifications.First(n => n.Id == notification.Id).SenderId);
                    notification.SenderName = sender?.FullName ?? "Người dùng không xác định";
                }
            }

            return new ApiResponse<IEnumerable<NotificationResponse>>(0, "Lấy danh sách thông báo thành công!",
                notificationResponses);
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<NotificationResponse>>(1, $"Có lỗi xảy ra: {ex.Message}", null);
        }
    }

    public async Task<ApiResponse<IEnumerable<NotificationResponse>>> GetNotificationsByUserAndTeachingAssignmentAsync(
        int userId,
        int teachingAssignmentId)
    {
        try
        {
            var notifications =
                await _notificationsRepository.GetNotificationsByUserAndTeachingAssignment(userId,
                    teachingAssignmentId);
            var notificationResponses = _mapper.Map<IEnumerable<NotificationResponse>>(notifications);

            foreach (var notification in notificationResponses)
            {
                if (notification.Type == "System")
                {
                    notification.SenderName = "Hệ thống";
                }
                else
                {
                    var sender =
                        await _context.Users.FindAsync(notifications.First(n => n.Id == notification.Id).SenderId);
                    notification.SenderName = sender?.FullName ?? "Người dùng không xác định";
                }
            }

            return new ApiResponse<IEnumerable<NotificationResponse>>(0,
                "Lấy danh sách thông báo theo phân công giảng dạy thành công!", notificationResponses);
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<NotificationResponse>>(1, $"Có lỗi xảy ra: {ex.Message}", null);
        }
    }

    public async Task<ApiResponse<IEnumerable<NotificationResponse>>> GetAllNotificationsByUserAsync(int userId)
    {
        try
        {
            var notifications = await _notificationsRepository.GetAllNotificationsByUser(userId);
            var notificationResponses = _mapper.Map<IEnumerable<NotificationResponse>>(notifications);

            foreach (var notification in notificationResponses)
            {
                if (notification.Type == "System")
                {
                    notification.SenderName = "Hệ thống";
                }
                else
                {
                    var sender =
                        await _context.Users.FindAsync(notifications.First(n => n.Id == notification.Id).SenderId);
                    notification.SenderName = sender?.FullName ?? "Người dùng không xác định";
                }
            }

            return new ApiResponse<IEnumerable<NotificationResponse>>(0,
                "Lấy danh sách thành công!", notificationResponses);
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<NotificationResponse>>(1, $"Đã có lỗi xảy ra {ex.Message}", null);
        }
    }

    public async Task SendSystemAnnouncementAsync(string subject, string content)
    {
        try
        {
            // Lấy danh sách tất cả người dùng
            var allUsers = await _context.Users
                .Where(u => u.IsDelete == false)
                .Select(u => u.Id)
                .ToListAsync();

            // Gửi thông báo đến tất cả người dùng
            foreach (var userId in allUsers)
            {
                await _notificationsRepository.AddNotification(
                    senderId: null, // Hệ thống
                    userId: userId,
                    subject: subject,
                    content: content,
                    type: true // Thông báo hệ thống
                );
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Không thể gửi thông báo hệ thống: {ex.Message}", ex);
        }
    }

    public async Task SendTeacherAnnouncementAsync(int teacherId, int classId, string subject, string content)
    {
        try
        {
            // Kiểm tra giảng viên
            var teacher = await _context.Users.FindAsync(teacherId);
            if (teacher == null)
            {
                throw new Exception("Giảng viên không tồn tại!");
            }

            // Lấy danh sách học sinh trong lớp
            var students = await _context.ClassStudents
                .Where(cs => cs.ClassId == classId && cs.IsDelete == false)
                .Select(cs => cs.UserId)
                .ToListAsync();

            // Gửi thông báo đến tất cả học sinh
            foreach (var studentId in students)
            {
                await _notificationsRepository.AddNotification(
                    senderId: null, // Hệ thống
                    userId: studentId.Value,
                    subject: subject,
                    content: content,
                    type: true // Thông báo hệ thống
                );
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Không thể gửi thông báo cho lớp: {ex.Message}", ex);
        }
    }

    public async Task<ApiResponse<NotificationResponse>> AddManualNotificationAsync(
        AddManualNotificationRequest request)
    {
        try
        {
            // Kiểm tra userId (người nhận) có tồn tại không
            var receiver = await _context.Users.FindAsync(request.UserId);
            if (receiver == null)
            {
                return new ApiResponse<NotificationResponse>(1, "Người nhận thông báo không tồn tại!", null);
            }

            // Kiểm tra senderId (người gửi) có tồn tại không
            var sender = await _context.Users.FindAsync(request.SenderId);
            if (sender == null)
            {
                return new ApiResponse<NotificationResponse>(1, "Người gửi thông báo không tồn tại!", null);
            }

            // Gọi phương thức kế thừa từ NotificationsRepository
            await _notificationsRepository.AddManualNotification(request.SenderId, request.UserId, request.Subject,
                request.Content);

            // Lấy thông báo vừa tạo để trả về
            var notification = await _context.Notifications
                .Where(n => n.UserId == request.UserId && n.SenderId == request.SenderId &&
                            n.Subject == request.Subject)
                .OrderByDescending(n => n.CreateAt)
                .FirstOrDefaultAsync();

            if (notification == null)
            {
                return new ApiResponse<NotificationResponse>(1, "Không thể tìm thấy thông báo vừa tạo!", null);
            }

            // Map sang NotificationResponse
            var notificationResponse = _mapper.Map<NotificationResponse>(notification);
            notificationResponse.SenderName = sender.FullName;

            return new ApiResponse<NotificationResponse>(0, "Gửi thông báo thành công!", notificationResponse);
        }
        catch (Exception ex)
        {
            return new ApiResponse<NotificationResponse>(1, $"Có lỗi xảy ra: {ex.Message}", null);
        }
    }

    public async Task<ApiResponse<bool>> DeleteNotificationAsync(DeleteRequest request, int userId)
    {
        try
        {
            if (request == null || request.ids == null || !request.ids.Any())
            {
                return new ApiResponse<bool>(1, "Danh sách ID thông báo không hợp lệ!", false);
            }

            var notifications = await _context.Notifications
                .Where(n => request.ids.Contains(n.Id) && n.UserId == userId).ToListAsync();
            if (!notifications.Any())
            {
                return new ApiResponse<bool>(1, "Không tìm thấy thông báo để xóa!", false);
            }

            foreach (var notification in notifications)
            {
                await _notificationsRepository.DeleteNotifications(notification.Id, userId);
            }

            return new ApiResponse<bool>(0, "Xóa thông báo thành công!", true);
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>(1, $"Có lỗi xảy ra: {ex.Message}", false);
        }
    }

    public async Task<ApiResponse<bool>> SelectIsReadAsync(DeleteRequest request, int userId)
    {
        try
        {
            if (request == null || request.ids == null || !request.ids.Any())
            {
                return new ApiResponse<bool>(1, "Danh sách ID thông báo không hợp lệ!", false);
            }

            var notification = await _context.Notifications
                .Where(n => request.ids.Contains(n.Id) && n.UserId == userId && n.IsDelete == false)
                .ToListAsync();

            if (!notification.Any())
            {
                return new ApiResponse<bool>(1, "Không tìm thấy thông báo để đánh dấu đọc!", false);
            }

            foreach (var notifications in notification)
            {
                await _notificationsRepository.SelectIsRead(notifications.Id, userId);
            }

            return new ApiResponse<bool>(0, "Đánh dấu đã đọc thành công!");
        }
        catch (Exception e)
        {
            return new ApiResponse<bool>(1, $"Đã có lỗi xảy ra {e.Message}");
        }
    }
}