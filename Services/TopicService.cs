using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Helpers;
using Project_LMS.Hubs;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services;

public class TopicService : ITopicService
{
    private readonly ITopicRepository _topicRepository;
    private readonly IMapper _mapper;
    private readonly ICloudinaryService _cloudinary;
    private readonly IHubContext<RealtimeHub> _hubContext;
    private readonly IUserRepository _userRepository;
    private readonly ITeachingAssignmentService _teachingAssignmentRepository;
    private readonly ApplicationDbContext _context;
    private readonly INotificationsService _notificationsService;

    public TopicService(ITopicRepository topicRepository, IMapper mapper, ICloudinaryService cloudinary,
        IHubContext<RealtimeHub> hubContext, ITeachingAssignmentService teachingAssignmentService,
        IUserRepository userRepository, ApplicationDbContext context, INotificationsService notificationsService)
    {
        _topicRepository = topicRepository;
        _mapper = mapper;
        _cloudinary = cloudinary;
        _hubContext = hubContext;
        _teachingAssignmentRepository = teachingAssignmentService;
        _userRepository = userRepository;
        _context = context;
        _notificationsService = notificationsService;
    }

    public async Task<ApiResponse<IEnumerable<TopicResponse>>> GetAllTopicsAsync(int userId, int teachingAssignmentId)
    {
        try
        {
            var topics = await _topicRepository.GetAllTopic(userId, teachingAssignmentId);
            return new ApiResponse<IEnumerable<TopicResponse>>(0, "Lấy danh sách topic thành công!", topics);
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<TopicResponse>>(1, $"Có lỗi xảy ra: {ex.Message}", null);
        }
    }

    public async Task<ApiResponse<TopicResponse>> GetTopicByIdAsync(int userId, int teachingAssignmentId, int id)
    {
        try
        {
            var topic = await _topicRepository.GetTopicById(userId, teachingAssignmentId, id);
            if (topic == null)
            {
                return new ApiResponse<TopicResponse>(1, "Không tìm thấy topic!", null);
            }

            // Ghi lượt xem nếu đây là topic gốc và user chưa xem
            if (topic.TopicId == null)
            {
                var existingView = await _context.QuestionAnswerTopicViews
                    .AnyAsync(qatv =>
                        qatv.TopicId == id && qatv.UserId == userId &&
                        (qatv.IsDelete == false || qatv.IsDelete == null));
                if (!existingView)
                {
                    var view = new QuestionAnswerTopicView
                    {
                        TopicId = id,
                        QuestionsAnswerId = null,
                        UserId = userId,
                        CreateAt = TimeHelper.NowUsingTimeZone,
                        UpdateAt = TimeHelper.NowUsingTimeZone,
                        UserCreate = userId,
                        UserUpdate = userId,
                        IsDelete = false
                    };
                    _context.QuestionAnswerTopicViews.Add(view);
                    await _context.SaveChangesAsync();

                    var teachingAssignment = await _context.TeachingAssignments.FindAsync(topic.TeachingAssignmentId);
                    if (teachingAssignment?.UserId.HasValue == true && teachingAssignment.UserId != userId)
                    {
                        await _hubContext.Clients.User(teachingAssignment.UserId.Value.ToString())
                            .SendAsync("ReceiveNotification", $"Topic (ID: {id}) đã được xem bởi user {userId}.");
                    }

                    // Cập nhật lại Views sau khi ghi lượt xem
                    topic.Views = await _context.QuestionAnswerTopicViews
                        .CountAsync(qatv => qatv.TopicId == id && (qatv.IsDelete == false || qatv.IsDelete == null));
                }
            }

            return new ApiResponse<TopicResponse>(0, "Lấy topic thành công!", topic);
        }
        catch (Exception ex)
        {
            return new ApiResponse<TopicResponse>(1, $"Có lỗi xảy ra: {ex.Message}", null);
        }
    }

    public async Task<ApiResponse<TopicResponse>> CreateTopicAsync(CreateTopicRequest request)
    {
        try
        {
            // 1) Kiểm tra Title (bắt buộc khi tạo topic gốc)
            if (!request.TopicId.HasValue && string.IsNullOrWhiteSpace(request.Title))
            {
                return new ApiResponse<TopicResponse>(1, "Tiêu đề là bắt buộc khi tạo topic!", null);
            }

            // 2) Kiểm tra UserId
            if (!request.UserId.HasValue || request.UserId <= 0)
            {
                return new ApiResponse<TopicResponse>(1, "UserId là bắt buộc và phải lớn hơn 0!", null);
            }

            // 3) Lấy thông tin user để kiểm tra vai trò
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return new ApiResponse<TopicResponse>(1, "Không tìm thấy user với Id đã cung cấp!", null);
            }

            // 4) Nếu là topic gốc, kiểm tra quyền tạo topic
            if (!request.TopicId.HasValue)
            {
                // Chỉ user có RoleId = 2 (giáo viên) được tạo topic
                if (user.RoleId != 2)
                {
                    return new ApiResponse<TopicResponse>(1, "Chỉ giáo viên (RoleId = 2) mới được tạo topic!", null);
                }

                // Kiểm tra TeachingAssignmentId
                if (request.TeachingAssignmentId == 0)
                {
                    return new ApiResponse<TopicResponse>(1, "TeachingAssignmentId là bắt buộc khi tạo topic!", null);
                }

                // Kiểm tra phân công giảng dạy
                var teachingAssignment = await _context.TeachingAssignments
                    .FirstOrDefaultAsync(ta => ta.Id == request.TeachingAssignmentId);
                if (teachingAssignment == null || teachingAssignment.IsDelete == true)
                {
                    return new ApiResponse<TopicResponse>(1, "Phân công giảng dạy không tồn tại!", null);
                }

                // Kiểm tra giáo viên thuộc phân công
                if (teachingAssignment.UserId != request.UserId)
                {
                    return new ApiResponse<TopicResponse>(1, "Bạn không được gán vào phân công giảng dạy này!", null);
                }
            }
            // 5) Nếu là comment, kiểm tra user có thuộc lớp học liên quan không
            else
            {
                // Lấy thông tin topic cha
                var parentTopic = await _topicRepository.GetTopicById(request.UserId.Value,
                    request.TeachingAssignmentId, request.TopicId.Value);
                if (parentTopic == null)
                {
                    return new ApiResponse<TopicResponse>(1, "Topic cha không tồn tại hoặc đã bị xóa!", null);
                }

                // Đảm bảo TeachingAssignmentId của comment khớp với topic cha
                if (!parentTopic.TeachingAssignmentId.HasValue)
                {
                    return new ApiResponse<TopicResponse>(1, "Topic cha không có TeachingAssignmentId hợp lệ!", null);
                }

                request.TeachingAssignmentId = parentTopic.TeachingAssignmentId.Value;
            }

            // 6) Map DTO -> Entity
            var topicEntity = _mapper.Map<Topic>(request);
            topicEntity.CreateAt = TimeHelper.NowUsingTimeZone;
            topicEntity.UpdateAt = TimeHelper.NowUsingTimeZone;
            topicEntity.UserCreate = request.UserId;
            topicEntity.UserUpdate = request.UserId;

            // 7) Kiểm tra thời gian closeAt (nếu có)
            if (topicEntity.CloseAt.HasValue)
            {
                var currentTime = TimeHelper.NowUsingTimeZone; // Thời điểm hiện tại
                var lowerBoundTime = currentTime.AddMinutes(-10); // Thời gian giới hạn dưới (hiện tại - 10 phút)

                if (topicEntity.CloseAt.Value < lowerBoundTime)
                {
                    return new ApiResponse<TopicResponse>(1,
                        $"Thời gian đóng (CloseAt) không hợp lệ: không được sớm hơn {lowerBoundTime:yyyy-MM-dd HH:mm:ss} (hiện tại - 10 phút)!",
                        null);
                }
            }

            // 8) Upload file nếu có
            if (!string.IsNullOrEmpty(request.FileName))
            {
                try
                {
                    topicEntity.FileName = await _cloudinary.UploadImageAsync(request.FileName);
                    if (string.IsNullOrWhiteSpace(topicEntity.FileName))
                    {
                        return new ApiResponse<TopicResponse>(1, "Tải file lên thất bại!", null);
                    }
                }
                catch (Exception ex)
                {
                    return new ApiResponse<TopicResponse>(1, $"Lỗi tải file: {ex.Message}", null);
                }
            }

            // 9) Lưu topic/comment vào DB (thông qua repository)
            var savedTopic = await _topicRepository.AddTopic(topicEntity);

            // 10) Map Entity -> Response DTO
            var topicResponse = _mapper.Map<TopicResponse>(savedTopic);

            // 11) Gửi thông báo
            if (!request.TopicId.HasValue) // Nếu là topic gốc
            {
                var teachingAssignment = await _context.TeachingAssignments
                    .FirstOrDefaultAsync(ta => ta.Id == request.TeachingAssignmentId);
                if (teachingAssignment != null && teachingAssignment.ClassId.HasValue)
                {
                    // Lấy danh sách học sinh trong lớp
                    var students = await _context.ClassStudents
                        .Where(cs => cs.ClassId == teachingAssignment.ClassId && cs.IsDelete == false)
                        .Select(cs => cs.UserId)
                        .ToListAsync();

                    var classInfo = await _context.Classes
                        .FirstOrDefaultAsync(c => c.Id == teachingAssignment.ClassId);

                    // Gửi thông báo cho từng học sinh
                    foreach (var studentId in students)
                    {
                        if (studentId != request.UserId) // Không gửi thông báo cho chính người tạo
                        {
                            await _notificationsService.AddNotificationAsync(
                                senderId: request.UserId, // Người tạo topic
                                userId: studentId.Value,
                                subject: "Topic mới trong lớp " + classInfo?.Name,
                                content:
                                $"{user.FullName} đã tạo topic mới '{request.Title}' trong lớp {classInfo?.Name}.",
                                type: false // Thông báo người dùng
                            );
                        }
                    }
                }
            }
            else // Nếu là comment
            {
                var parentTopic = await _topicRepository.GetTopicById(request.UserId.Value,
                    request.TeachingAssignmentId, request.TopicId.Value);
                if (parentTopic != null && parentTopic.UserId != request.UserId)
                {
                    await _notificationsService.AddNotificationAsync(
                        senderId: request.UserId, // Người comment
                        userId: parentTopic.UserId.Value, // Giáo viên (người tạo topic)
                        subject: "Bình luận mới trong topic của bạn",
                        content:
                        $"{user.FullName} đã bình luận trong topic '{parentTopic.Title}': {request.Description}",
                        type: false // Thông báo người dùng
                    );
                }
            }

            // 12) Gửi sự kiện SignalR cho các client khác
            string message = request.TopicId.HasValue
                ? "Có comment mới trong topic!"
                : "Có topic mới được tạo!";
            await _hubContext.Clients.AllExcept(new[] { topicResponse.UserId.ToString() })
                .SendAsync("TopicCreated", message, topicResponse);

            // 13) Trả về phản hồi
            string successMessage = request.TopicId.HasValue
                ? "Thêm comment thành công!"
                : "Thêm topic thành công!";
            return new ApiResponse<TopicResponse>(0, successMessage, topicResponse);
        }
        catch (Exception ex)
        {
            return new ApiResponse<TopicResponse>(1, $"Có lỗi xảy ra: {ex.Message}", null);
        }
    }

    public async Task<ApiResponse<TopicResponse>> UpdateTopicAsync(UpdateTopicRequest request)
    {
        try
        {
            // 1) Kiểm tra Id
            if (request.Id <= 0)
            {
                return new ApiResponse<TopicResponse>(1, "Id là bắt buộc và phải lớn hơn 0!", null);
            }

            // 2) Kiểm tra UserId
            if (!request.UserId.HasValue || request.UserId <= 0)
            {
                return new ApiResponse<TopicResponse>(1, "UserId là bắt buộc và phải lớn hơn 0!", null);
            }

            // 3) Lấy thông tin topic hiện tại để kiểm tra
            var existingTopic = await _topicRepository.GetTopicById(request.UserId.Value,
                request.TeachingAssignmentId.Value, request.Id);
            if (existingTopic == null)
            {
                return new ApiResponse<TopicResponse>(1, "Topic không tồn tại hoặc đã bị xóa!", null);
            }

            // 4) Lấy thông tin user để kiểm tra vai trò
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return new ApiResponse<TopicResponse>(1, "Không tìm thấy user với Id đã cung cấp!", null);
            }

            // 5) Nếu là topic (TopicId == null), chỉ giáo viên (RoleId = 2) được cập nhật
            if (!existingTopic.TopicId.HasValue)
            {
                if (user.RoleId != 2)
                {
                    return new ApiResponse<TopicResponse>(1, "Chỉ giáo viên (RoleId = 2) mới được cập nhật topic!",
                        null);
                }
            }

            // 6) Nếu là comment, kiểm tra user có quyền cập nhật không
            if (existingTopic.TopicId.HasValue)
            {
                // Nếu user không phải giáo viên (RoleId != 2), chỉ được cập nhật comment của chính mình
                if (user.RoleId != 2 && existingTopic.UserId != request.UserId)
                {
                    return new ApiResponse<TopicResponse>(1, "Bạn chỉ được cập nhật comment của chính mình!", null);
                }

                // Lấy thông tin topic cha để lấy TeachingAssignmentId
                var parentTopic = await _topicRepository.GetTopicById(request.UserId.Value,
                    request.TeachingAssignmentId.Value, existingTopic.TopicId.Value);
                if (parentTopic == null)
                {
                    return new ApiResponse<TopicResponse>(1, "Topic cha không tồn tại hoặc đã bị xóa!", null);
                }

                if (!parentTopic.TeachingAssignmentId.HasValue)
                {
                    return new ApiResponse<TopicResponse>(1, "Topic cha không có TeachingAssignmentId hợp lệ!", null);
                }

                request.TeachingAssignmentId = parentTopic.TeachingAssignmentId.Value;
            }
            else
            {
                // Nếu là topic, kiểm tra TeachingAssignmentId
                if (!request.TeachingAssignmentId.HasValue)
                {
                    return new ApiResponse<TopicResponse>(1, "TeachingAssignmentId là bắt buộc khi cập nhật topic!",
                        null);
                }
            }

            // 7) Map DTO -> Entity
            var topicEntity = _mapper.Map<Topic>(request);

            // 8) Kiểm tra thời gian closeAt (nếu có)
            if (topicEntity.CloseAt.HasValue)
            {
                var currentTime = TimeHelper.NowUsingTimeZone; // Thời điểm hiện tại
                var lowerBoundTime = currentTime.AddMinutes(-10); // Thời gian giới hạn dưới (hiện tại - 10 phút)

                if (topicEntity.CloseAt.Value < lowerBoundTime)
                {
                    return new ApiResponse<TopicResponse>(1,
                        $"Thời gian đóng (CloseAt) không hợp lệ: không được sớm hơn {lowerBoundTime:yyyy-MM-dd HH:mm:ss} (hiện tại - 10 phút)!",
                        null);
                }
            }

            // 9) Xử lý file: Xóa file cũ và upload file mới nếu có
            if (!string.IsNullOrEmpty(request.FileName))
            {
                try
                {
                    // Xóa file cũ trên Cloudinary nếu có
                    if (!string.IsNullOrEmpty(existingTopic.FileName))
                    {
                        await _cloudinary.DeleteFileByUrlAsync(existingTopic.FileName);
                    }

                    // Upload file mới
                    topicEntity.FileName = await _cloudinary.UploadImageAsync(request.FileName);
                    if (string.IsNullOrWhiteSpace(topicEntity.FileName))
                    {
                        return new ApiResponse<TopicResponse>(1, "Tải file lên thất bại!", null);
                    }
                }
                catch (Exception ex)
                {
                    return new ApiResponse<TopicResponse>(1, $"Lỗi xử lý file: {ex.Message}", null);
                }
            }

            // 10) Gọi repository để cập nhật topic/comment
            var updatedTopic = await _topicRepository.UpdateTopic(topicEntity);
            if (updatedTopic == null)
            {
                return new ApiResponse<TopicResponse>(1, "Topic không tồn tại hoặc đã bị xóa!", null);
            }

            // 11) Map Entity -> Response DTO
            var topicResponse = _mapper.Map<TopicResponse>(updatedTopic);

            // 12) Gửi thông báo
            if (!request.TopicId.HasValue) // Nếu là topic gốc
            {
                var teachingAssignment = await _context.TeachingAssignments
                    .FirstOrDefaultAsync(ta => ta.Id == request.TeachingAssignmentId);
                if (teachingAssignment != null && teachingAssignment.ClassId.HasValue)
                {
                    // Lấy danh sách học sinh trong lớp
                    var students = await _context.ClassStudents
                        .Where(cs => cs.ClassId == teachingAssignment.ClassId && cs.IsDelete == false)
                        .Select(cs => cs.UserId)
                        .ToListAsync();

                    var classInfo = await _context.Classes
                        .FirstOrDefaultAsync(c => c.Id == teachingAssignment.ClassId);

                    // Gửi thông báo cho từng học sinh
                    foreach (var studentId in students)
                    {
                        if (studentId != request.UserId) // Không gửi thông báo cho chính người cập nhật
                        {
                            await _notificationsService.AddNotificationAsync(
                                senderId: request.UserId, // Người cập nhật topic
                                userId: studentId.Value,
                                subject: "Topic đã được cập nhật!",
                                content:
                                $"{user.FullName} đã cập nhật topic '{request.Title}' trong lớp {classInfo?.Name}.",
                                type: false // Thông báo người dùng
                            );
                        }
                    }
                }
            }
            else // Nếu là comment
            {
                var parentTopic = await _topicRepository.GetTopicById(request.UserId.Value,
                    request.TeachingAssignmentId.Value, request.TopicId.Value);
                if (parentTopic != null && parentTopic.UserId != request.UserId)
                {
                    await _notificationsService.AddNotificationAsync(
                        senderId: request.UserId, // Người cập nhật comment
                        userId: parentTopic.UserId.Value,
                        subject: "Comment trong topic của bạn đã được cập nhật!",
                        content: $"{user.FullName} đã cập nhật comment trong topic '{parentTopic.Title}'.",
                        type: false // Thông báo người dùng
                    );
                }
            }

            // 13) Gửi sự kiện SignalR cho các client khác
            string message = request.TopicId.HasValue
                ? "Có comment trong topic được cập nhật!"
                : "Có topic được cập nhật!";
            await _hubContext.Clients.AllExcept(new[] { topicResponse.UserId.ToString() })
                .SendAsync("TopicUpdated", message, topicResponse);

            // 14) Trả về phản hồi
            string successMessage = request.TopicId.HasValue
                ? "Cập nhật comment thành công!"
                : "Cập nhật topic thành công!";
            return new ApiResponse<TopicResponse>(0, successMessage, topicResponse);
        }
        catch (Exception ex)
        {
            return new ApiResponse<TopicResponse>(1, $"Có lỗi xảy ra: {ex.Message}", null);
        }
    }


    public async Task<ApiResponse<bool>> DeleteTopicAsync(int userId, int teachingAssignmentId, int id)
    {
        try
        {
            // 1) Lấy thông tin topic để kiểm tra và gửi thông báo
            var topic = await _topicRepository.GetTopicById(userId, teachingAssignmentId, id);
            if (topic == null)
            {
                return new ApiResponse<bool>(1, "Topic không tồn tại hoặc đã bị xóa!", false);
            }

            // 2) Lấy thông tin user để kiểm tra vai trò
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ApiResponse<bool>(1, "Không tìm thấy user với Id đã cung cấp!", false);
            }

            // 3) Kiểm tra quyền xóa
            if (!topic.TopicId.HasValue) // Nếu là topic gốc
            {
                // Chỉ giáo viên (RoleId = 2) được xóa topic
                if (user.RoleId != 2)
                {
                    return new ApiResponse<bool>(1, "Chỉ giáo viên (RoleId = 2) mới được xóa topic!", false);
                }
            }
            else // Nếu là comment
            {
                // Nếu user không phải giáo viên (RoleId != 2), chỉ được xóa comment của chính mình
                if (user.RoleId != 2 && topic.UserId != userId)
                {
                    return new ApiResponse<bool>(1, "Bạn chỉ được xóa comment của chính mình!", false);
                }
            }

            // 4) Xóa topic
            var success = await _topicRepository.DeleteTopic(userId, teachingAssignmentId, id);
            if (!success)
            {
                return new ApiResponse<bool>(1, "Xóa topic thất bại!", false);
            }

            // 5) Gửi thông báo
            var teachingAssignment = await _context.TeachingAssignments
                .FirstOrDefaultAsync(ta => ta.Id == topic.TeachingAssignmentId);
            var classInfo = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == teachingAssignment.ClassId);

            if (!topic.TopicId.HasValue) // Nếu là topic gốc
            {
                if (teachingAssignment?.ClassId.HasValue == true)
                {
                    // Lấy danh sách học sinh trong lớp
                    var students = await _context.ClassStudents
                        .Where(cs => cs.ClassId == teachingAssignment.ClassId && cs.IsDelete == false)
                        .Select(cs => cs.UserId)
                        .ToListAsync();

                    // Gửi thông báo cho từng học sinh
                    foreach (var studentId in students)
                    {
                        if (studentId != userId) // Không gửi thông báo cho chính người xóa
                        {
                            await _notificationsService.AddNotificationAsync(
                                senderId: userId, // Người xóa topic
                                userId: studentId.Value,
                                subject: "Topic đã bị xóa!",
                                content: $"{user.FullName} đã xóa topic '{topic.Title}' trong lớp {classInfo?.Name}.",
                                type: false // Thông báo người dùng
                            );
                        }
                    }
                }
            }
            else // Nếu là comment
            {
                var parentTopic =
                    await _topicRepository.GetTopicById(userId, teachingAssignmentId, topic.TopicId.Value);
                if (parentTopic != null && parentTopic.UserId != userId)
                {
                    await _notificationsService.AddNotificationAsync(
                        senderId: userId, // Người xóa comment
                        userId: parentTopic.UserId.Value,
                        subject: "Comment trong topic của bạn đã bị xóa!",
                        content: $"{user.FullName} đã xóa một comment trong topic '{parentTopic.Title}'.",
                        type: false // Thông báo người dùng
                    );
                }
            }

            // 6) Gửi sự kiện SignalR
            await _hubContext.Clients.All.SendAsync("TopicDeleted", id);

            return new ApiResponse<bool>(0, "Xóa topic thành công!", true);
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>(1, $"Có lỗi xảy ra: {ex.Message}", false);
        }
    }

    public async Task<ApiResponse<IEnumerable<TopicResponse>>> SearchTopicsAsync(int userId, int teachingAssignmentId,
        string? keyword)
    {
        try
        {
            var topics = await _topicRepository.SearchTopic(userId, teachingAssignmentId, keyword);
            if (topics == null || !topics.Any())
            {
                return new ApiResponse<IEnumerable<TopicResponse>>(0, "Không tìm thấy topic!",
                    new List<TopicResponse>());
            }

            return new ApiResponse<IEnumerable<TopicResponse>>(0, "Tìm kiếm topic thành công!", topics);
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<TopicResponse>>(1, $"Có lỗi xảy ra: {ex.Message}", null);
        }
    }

    public async Task<ApiResponse<bool>> SendTopicMessageAsync(int senderId, int teachingAssignmentId, int receiverId,
        int topicId, string message)
    {
        try
        {
            // 1) Kiểm tra người gửi và người nhận
            var sender = await _context.Users.FindAsync(senderId);
            var receiver = await _context.Users.FindAsync(receiverId);
            if (sender == null || receiver == null)
            {
                return new ApiResponse<bool>(1, "Người gửi hoặc người nhận không tồn tại!", false);
            }

            // 2) Kiểm tra topic
            var topic = await _topicRepository.GetTopicById(senderId, teachingAssignmentId, topicId);
            if (topic == null)
            {
                return new ApiResponse<bool>(1, "Topic không tồn tại hoặc đã bị xóa!", false);
            }

            // 3) Gửi thông báo người dùng
            await _notificationsService.AddNotificationAsync(
                senderId: senderId, // Người gửi
                userId: receiverId, // Người nhận
                subject: $"Tin nhắn từ {sender.FullName} về topic '{topic.Title}'",
                content: message,
                type: false // Thông báo người dùng
            );

            // 4) Gửi thông báo realtime
            await _hubContext.Clients.User(receiverId.ToString())
                .SendAsync("ReceiveNotification",
                    $"Bạn có tin nhắn mới từ {sender.FullName} về topic '{topic.Title}': {message}");

            return new ApiResponse<bool>(0, "Gửi tin nhắn thành công!", true);
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>(1, $"Có lỗi xảy ra: {ex.Message}", false);
        }
    }
}