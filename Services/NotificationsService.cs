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
    private readonly NotificationQueueService _notificationQueueService;

    public NotificationsService(INotificationsRepository notificationsRepository, IMapper mapper,
        ApplicationDbContext context, NotificationQueueService notificationQueueService)
    {
        _notificationsRepository = notificationsRepository;
        _mapper = mapper;
        _context = context;
        _notificationQueueService = notificationQueueService;
    }

    /*Chức năng: Gửi thông báo đến nhiều người dùng cùng lúc.*/
    public async Task AddNotificationAsync(int? senderId, int userId, string subject, string content, bool type)
    {
        try
        {
            // Thay vì gọi trực tiếp, chúng ta đưa vào queue
            _notificationQueueService.QueueNotification(new NotificationQueueItem
            {
                SenderId = senderId,
                UserId = userId,
                Subject = subject,
                Content = content,
                Type = type
            });
        }
        catch (Exception ex)
        {
            throw new Exception("Không thể gửi thông báo, vui lòng kiểm tra lại thông tin người nhận");
        }
    }

    public async Task AddNotificationToUsersAsync(List<int> userIds, string subject, string content)
    {
        try
        {
            _notificationQueueService.QueueNotificationToUsers(null, userIds, subject, content, false);
        }
        catch (Exception ex)
        {
            throw new Exception($"Không thể thêm thông báo đến danh sách người dùng: {ex.Message}", ex);
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

            // Gửi thông báo đến tất cả người dùng qua hàng đợi
            _notificationQueueService.QueueNotificationToUsers(null, allUsers, subject, content, true);
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
                .Where(cs => cs.ClassId == classId && cs.IsDelete == false && cs.UserId.HasValue)
                .Select(cs => cs.UserId.Value)
                .ToListAsync();

            // Gửi thông báo đến tất cả học sinh qua hàng đợi
            _notificationQueueService.QueueNotificationToUsers(null, students, subject, content, true);
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

    public async Task<ApiResponse<bool>> SendClassNotificationAsync(int senderUserId, int classId, string subject, string content, bool? type = false)
    {
        try
        {
            // Kiểm tra quyền hạn người gửi (phải là admin hoặc giáo viên)
            var sender = await _context.Users.FindAsync(senderUserId);
            if (sender == null)
            {
                return new ApiResponse<bool>(1, "Người gửi không tồn tại!", false);
            }
            
            if (sender.RoleId != 1 && sender.RoleId != 2) // Không phải Admin hoặc giáo viên
            {
                return new ApiResponse<bool>(1, "Chỉ Admin hoặc giáo viên mới có quyền gửi thông báo!", false);
            }
            
            // Kiểm tra lớp học tồn tại
            var classInfo = await _context.Classes.FindAsync(classId);
            if (classInfo == null || classInfo.IsDelete == true)
            {
                return new ApiResponse<bool>(1, "Lớp học không tồn tại!", false);
            }
            
            // Lấy danh sách học sinh trong lớp
            var studentIds = await _context.ClassStudents
                .Where(cs => cs.ClassId == classId && cs.IsDelete == false && cs.IsActive == true && cs.UserId.HasValue)
                .Select(cs => cs.UserId.Value)
                .ToListAsync();
            
            // Lấy danh sách giáo viên dạy lớp này
            var teacherIds = await _context.TeachingAssignments
                .Where(ta => ta.ClassId == classId && ta.IsDelete == false && ta.UserId.HasValue)
                .Select(ta => ta.UserId.Value)
                .ToListAsync();
            
            // Thêm giáo viên chủ nhiệm nếu có
            if (classInfo.UserId.HasValue && !teacherIds.Contains(classInfo.UserId.Value))
            {
                teacherIds.Add(classInfo.UserId.Value);
            }
            
            // Gộp danh sách người nhận
            var receiverIds = studentIds.Union(teacherIds).Distinct().ToList();
            
            if (!receiverIds.Any())
            {
                return new ApiResponse<bool>(1, "Không tìm thấy người nhận trong lớp này!", false);
            }
            
            // Tùy chỉnh nội dung thông báo
            var formattedContent = $"{sender.FullName} đã gửi thông báo '{subject}' cho lớp {classInfo.Name}. Nội dung: {content}";
            
            // Gửi thông báo đến tất cả người nhận qua hàng đợi
            _notificationQueueService.QueueNotificationToUsers(
                type == true ? null : senderUserId, 
                receiverIds, 
                subject, 
                formattedContent, 
                type ?? false
            );
            
            return new ApiResponse<bool>(0, $"Đã gửi thông báo đến {receiverIds.Count} người dùng trong lớp {classInfo.Name}!", true);
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>(1, $"Có lỗi xảy ra: {ex.Message}", false);
        }
    }

    public async Task<ApiResponse<bool>> SendUserListNotificationAsync(int senderUserId, List<int> userIds, string subject, string content, bool? type = false)
    {
        try
        {
            // Kiểm tra quyền hạn người gửi (phải là admin hoặc giáo viên)
            var sender = await _context.Users.FindAsync(senderUserId);
            if (sender == null)
            {
                return new ApiResponse<bool>(1, "Người gửi không tồn tại!", false);
            }
            
            if (sender.RoleId != 1 && sender.RoleId != 2) // Không phải Admin hoặc giáo viên
            {
                return new ApiResponse<bool>(1, "Chỉ Admin hoặc giáo viên mới có quyền gửi thông báo!", false);
            }
            
            // Kiểm tra danh sách người nhận
            if (userIds == null || !userIds.Any())
            {
                return new ApiResponse<bool>(1, "Danh sách người nhận không được rỗng!", false);
            }
            
            // Kiểm tra người nhận tồn tại trong hệ thống
            var existingUserIds = await _context.Users
                .Where(u => userIds.Contains(u.Id) && u.IsDelete == false)
                .Select(u => u.Id)
                .ToListAsync();
            
            if (existingUserIds.Count != userIds.Count)
            {
                return new ApiResponse<bool>(1, "Có người dùng không tồn tại trong hệ thống!", false);
            }
            
            // Tùy chỉnh nội dung thông báo
            string formattedContent;
            if (type == true)
            {
                // Thông báo hệ thống
                formattedContent = content;
            }
            else
            {
                // Thông báo từ người dùng
                formattedContent = $"{sender.FullName} đã gửi thông báo '{subject}'. Nội dung: {content}";
            }
            
            // Gửi thông báo đến tất cả người nhận qua hàng đợi
            _notificationQueueService.QueueNotificationToUsers(
                type == true ? null : senderUserId,
                userIds,
                subject,
                formattedContent,
                type ?? false
            );
            
            return new ApiResponse<bool>(0, "Gửi thông báo thành công!", true);
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>(1, $"Đã có lỗi xảy ra: {ex.Message}", false);
        }
    }
}