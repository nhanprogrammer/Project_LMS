using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface INotificationsService
    {
        Task AddNotificationAsync(int? senderId, int userId, string subject, string content, bool type);
        Task<ApiResponse<IEnumerable<NotificationResponse>>> GetNotificationsByUserIdAsync(int userId);

        Task<ApiResponse<IEnumerable<NotificationResponse>>> GetNotificationsByUserAndTeachingAssignmentAsync(
            int userId,
            int teachingAssignmentId);

        Task<ApiResponse<IEnumerable<NotificationResponse>>>
            GetAllNotificationsByUserAsync(int userId); // Phương thức mới

        Task SendSystemAnnouncementAsync(string subject, string content); // Thông báo hệ thống cho tất cả người dùng

        Task SendTeacherAnnouncementAsync(int teacherId, int classId, string subject,
            string content); // Thông báo hệ thống cho lớp học

        Task<ApiResponse<NotificationResponse>> AddManualNotificationAsync(AddManualNotificationRequest request);
        Task<ApiResponse<bool>> DeleteNotificationAsync(DeleteRequest request, int userId); // Thêm phương thức xóa
        Task<ApiResponse<bool>> SelectIsReadAsync(DeleteRequest request, int userId); // Thêm phương thức
    }
}