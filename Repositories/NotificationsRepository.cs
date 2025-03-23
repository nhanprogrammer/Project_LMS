using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Hubs;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories;

public sealed class NotificationsRepository : INotificationsRepository
{
    private readonly ApplicationDbContext _context; // protected để lớp con truy cập
    private readonly IHubContext<RealtimeHub> _hubContext; // protected để lớp con truy cập

    public NotificationsRepository(ApplicationDbContext context, IHubContext<RealtimeHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task AddNotification(int? senderId, int userId, string subject, string content, bool type)
    {
        try
        {
            var notification = new Notification
            {
                SenderId = type ? null : senderId,
                UserId = userId,
                Subject = subject,
                Content = content,
                Type = type,
                IsRead = false,
                IsDelete = false,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                UserCreate = senderId ?? 0,
                UserUpdate = senderId ?? 0
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(userId.ToString())
                .SendAsync("ReceiveNotification", new
                {
                    Id = notification.Id,
                    Subject = notification.Subject,
                    Content = notification.Content,
                    CreateAt = notification.CreateAt.ToString(),
                    Type = notification.Type.Value ? "System" : "User"
                });
        }
        catch (Exception ex)
        {
            throw new Exception($"Không thể thêm thông báo: {ex.Message}", ex);
        }
    }

    public async Task AddManualNotification(int senderId, int userId, string subject, string content)
    {
        try
        {
            // Kiểm tra userId (người nhận) có tồn tại không
            var receiver = await _context.Users.FindAsync(userId);
            if (receiver == null)
            {
                throw new Exception("Người nhận thông báo không tồn tại!");
            }

            // Kiểm tra senderId (người gửi) có tồn tại không
            var sender = await _context.Users.FindAsync(senderId);
            if (sender == null)
            {
                throw new Exception("Người gửi thông báo không tồn tại!");
            }

            // Chỉ giáo viên (RoleId = 2) được phép gửi thông báo từ giao diện
            if (sender.RoleId != 2)
            {
                throw new Exception("Chỉ giáo viên (RoleId = 2) mới được phép gửi thông báo từ giao diện!");
            }

            // Kiểm tra xem học sinh có thuộc lớp mà giáo viên phụ trách không
            // Bước 1: Lấy danh sách phân công giảng dạy của giáo viên (senderId)
            var teachingAssignments = await _context.TeachingAssignments
                .Where(ta => ta.UserId == senderId && ta.IsDelete == false)
                .Select(ta => new { ta.ClassId })
                .ToListAsync();

            if (!teachingAssignments.Any())
            {
                throw new Exception("Giáo viên không có phân công giảng dạy nào!");
            }

            // Bước 2: Kiểm tra xem học sinh (userId) có thuộc lớp mà giáo viên phụ trách không
            var studentClass = await _context.ClassStudents
                .Where(cs =>
                    cs.UserId == userId && teachingAssignments.Select(ta => ta.ClassId).Contains(cs.ClassId) &&
                    cs.IsDelete == false)
                .Join(_context.Classes,
                    cs => cs.ClassId,
                    c => c.Id,
                    (cs, c) => new { ClassName = c.Name })
                .FirstOrDefaultAsync();

            if (studentClass == null)
            {
                throw new Exception("Học sinh không thuộc lớp mà giáo viên phụ trách!");
            }

            // Tùy biến nội dung thông báo
            var formattedContent =
                $"{sender.FullName} đã gửi thông báo '{subject}' cho bạn trong lớp {studentClass.ClassName}.";

            // Tạo thông báo (luôn là thông báo người dùng, type = false)
            var notification = new Notification
            {
                SenderId = senderId,
                UserId = userId,
                Subject = subject,
                Content = formattedContent, // Sử dụng nội dung đã tùy biến
                Type = false,
                IsRead = false,
                IsDelete = false,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                UserCreate = senderId,
                UserUpdate = senderId
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Gửi thông báo realtime
            await _hubContext.Clients.User(userId.ToString())
                .SendAsync("ReceiveNotification", new
                {
                    Id = notification.Id,
                    Subject = notification.Subject,
                    Content = notification.Content,
                    CreateAt = notification.CreateAt.ToString(),
                    Type = "User" // Luôn là thông báo người dùng
                });
        }
        catch (Exception ex)
        {
            throw new Exception($"Không thể thêm thông báo từ giao diện: {ex.Message}", ex);
        }
    }

    public async Task DeleteNotifications(int notificationId, int userId)
    {
        try
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(n =>
                n.Id == notificationId && n.UserId == userId && n.IsDelete == false);
            if (notification == null)
            {
                throw new Exception("Thông báo không tồn tại hoặc bạn không có quyền xóa!");
            }

            notification.IsDelete = true;
            notification.UpdateAt = DateTime.UtcNow;
            notification.UserUpdate = userId;

            await _context.SaveChangesAsync();
            await _hubContext.Clients.User(userId.ToString()).SendAsync("NotificationDeleted", notification);
        }
        catch (Exception ex)
        {
            throw new Exception($"Không thể xóa thông báo: {ex.Message}", ex);
        }
    }

    public async Task SelectIsRead(int notificationId, int userId)
    {
        try
        {
            // Tìm thông báo
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId && n.IsDelete == false);

            if (notification == null)
            {
                throw new Exception("Thông báo không tồn tại hoặc đã bị xóa!");
            }

            // Cập nhật trạng thái đã đọc
            notification.IsRead = true;
            notification.UpdateAt = DateTime.UtcNow;
            notification.UserUpdate = userId;

            await _context.SaveChangesAsync();

            // Gửi tín hiệu qua SignalR để cập nhật trạng thái đã đọc
            await _hubContext.Clients.User(userId.ToString())
                .SendAsync("NotificationIsRead", new
                {
                    Id = notification.Id,
                    IsRead = notification.IsRead
                });
        }
        catch (Exception ex)
        {
            throw new Exception($"Không thể đánh dấu thông báo là đã đọc: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByUserId(int userId)
    {
        try
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && n.IsDelete == false)
                .OrderByDescending(n => n.CreateAt)
                .ToListAsync();

            return notifications;
        }
        catch (Exception ex)
        {
            throw new Exception($"Không thể lấy danh sách thông báo: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByUserAndTeachingAssignment(int userId,
        int teachingAssignmentId)
    {
        try
        {
            // Kiểm tra xem phân công giảng dạy có tồn tại không
            var teachingAssignment = await _context.TeachingAssignments
                .FirstOrDefaultAsync(ta => ta.Id == teachingAssignmentId && ta.IsDelete == false);

            if (teachingAssignment == null)
            {
                throw new Exception("Phân công giảng dạy không tồn tại!");
            }

            // Lấy thông báo của giáo viên
            var teacherNotifications = await _context.Notifications
                .Join(_context.Users,
                    n => n.UserId,
                    u => u.Id,
                    (n, u) => new { Notification = n, User = u })
                .Join(_context.TeachingAssignments,
                    nu => nu.User.Id,
                    ta => ta.UserId,
                    (nu, ta) => new { nu.Notification, TeachingAssignment = ta })
                .Where(nuta =>
                    nuta.TeachingAssignment.Id == teachingAssignmentId && nuta.Notification.IsDelete == false &&
                    nuta.Notification.UserId == userId)
                .Select(nuta => nuta.Notification)
                .ToListAsync();

            // Lấy thông báo của học sinh
            var studentNotifications = await _context.Notifications
                .Join(_context.Users,
                    n => n.UserId,
                    u => u.Id,
                    (n, u) => new { Notification = n, User = u })
                .Join(_context.ClassStudents,
                    nu => nu.User.Id,
                    cs => cs.UserId,
                    (nu, cs) => new { nu.Notification, ClassStudent = cs })
                .Join(_context.TeachingAssignments,
                    nucs => nucs.ClassStudent.ClassId,
                    ta => ta.ClassId,
                    (nucs, ta) => new { nucs.Notification, TeachingAssignment = ta })
                .Where(nuta =>
                    nuta.TeachingAssignment.Id == teachingAssignmentId && nuta.Notification.IsDelete == false &&
                    nuta.Notification.UserId == userId)
                .Select(nuta => nuta.Notification)
                .ToListAsync();

            // Kết hợp thông báo của giáo viên và học sinh
            var allNotifications = teacherNotifications
                .Union(studentNotifications)
                .OrderByDescending(n => n.CreateAt)
                .ToList();

            return allNotifications;
        }
        catch (Exception ex)
        {
            throw new Exception($"Không thể lấy danh sách thông báo: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<Notification>> GetAllNotificationsByUser(int userId)
    {
        try
        {
            // Lấy tất cả teachingAssignmentId của giáo viên
            var teacherTeachingAssignments = await _context.TeachingAssignments
                .Where(ta => ta.UserId == userId && ta.IsDelete == false)
                .Select(ta => ta.Id)
                .ToListAsync();

            // Lấy tất cả teachingAssignmentId của học sinh
            var studentTeachingAssignments = await _context.ClassStudents
                .Where(cs => cs.UserId == userId && cs.IsDelete == false)
                .Join(_context.TeachingAssignments,
                    cs => cs.ClassId,
                    ta => ta.ClassId,
                    (cs, ta) => ta.Id)
                .Where(taId => !_context.TeachingAssignments.Any(ta => ta.Id == taId && ta.IsDelete == true))
                .ToListAsync();

            // Kết hợp danh sách teachingAssignmentId
            var allTeachingAssignmentIds = teacherTeachingAssignments
                .Union(studentTeachingAssignments)
                .Distinct()
                .ToList();

            // Lấy thông báo từ tất cả teachingAssignmentId
            var allNotifications = new List<Notification>();
            foreach (var teachingAssignmentId in allTeachingAssignmentIds)
            {
                var notifications = await _context.Notifications
                    .Join(_context.Users,
                        n => n.UserId,
                        u => u.Id,
                        (n, u) => new { Notification = n, User = u })
                    .Join(_context.TeachingAssignments,
                        nu => nu.User.Id,
                        ta => ta.UserId,
                        (nu, ta) => new { nu.Notification, TeachingAssignment = ta })
                    .Where(nuta =>
                        nuta.TeachingAssignment.Id == teachingAssignmentId && nuta.Notification.IsDelete == false &&
                        nuta.Notification.UserId == userId)
                    .Select(nuta => nuta.Notification)
                    .Union(
                        _context.Notifications
                            .Join(_context.Users,
                                n => n.UserId,
                                u => u.Id,
                                (n, u) => new { Notification = n, User = u })
                            .Join(_context.ClassStudents,
                                nu => nu.User.Id,
                                cs => cs.UserId,
                                (nu, cs) => new { nu.Notification, ClassStudent = cs })
                            .Join(_context.TeachingAssignments,
                                nucs => nucs.ClassStudent.ClassId,
                                ta => ta.ClassId,
                                (nucs, ta) => new { nucs.Notification, TeachingAssignment = ta })
                            .Where(nuta =>
                                nuta.TeachingAssignment.Id == teachingAssignmentId &&
                                nuta.Notification.IsDelete == false && nuta.Notification.UserId == userId)
                            .Select(nuta => nuta.Notification)
                    )
                    .ToListAsync();

                allNotifications.AddRange(notifications);
            }

            // Sắp xếp theo CreateAt giảm dần
            allNotifications = allNotifications
                .OrderByDescending(n => n.CreateAt)
                .ToList();

            return allNotifications;
        }
        catch (Exception ex)
        {
            throw new Exception($"Không thể lấy danh sách thông báo: {ex.Message}", ex);
        }
    }
}