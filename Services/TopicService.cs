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

    public async Task<ApiResponse<IEnumerable<TopicResponse>>> GetAllTopicsAsync()
    {
        var topics = await _topicRepository.GetAllTopic();
        return new ApiResponse<IEnumerable<TopicResponse>>(0, "Lấy danh sách topic thành công!", topics);
    }

    public async Task<ApiResponse<TopicResponse>> GetTopicByIdAsync(int id, int? userId)
    {
        var topic = await _topicRepository.GetTopicById(id);
        if (topic == null)
        {
            return new ApiResponse<TopicResponse>(1, "Không tìm thấy topic!", null);
        }

        // Kiểm tra quyền truy cập nếu userId được cung cấp
        if (userId.HasValue)
        {
            var teachingAssignment = await _context.TeachingAssignments.FindAsync(topic.TeachingAssignmentId);
            if (teachingAssignment == null || teachingAssignment.IsDelete == true)
            {
                return new ApiResponse<TopicResponse>(1, "Phân công giảng dạy không tồn tại!", null);
            }

            bool isUserInClass =
                await _topicRepository.IsUserInClassAsync(userId.Value, teachingAssignment.ClassId ?? 1);
            var classMembers = await _context.Classes
                .Where(c => c.Id == teachingAssignment.ClassId).FirstOrDefaultAsync();
            if (!isUserInClass)
            {
                return new ApiResponse<TopicResponse>(1,
                    $"Bạn không thuộc lớp học {classMembers?.Name} để xem topic này!", null);
            }

            // Ghi lượt xem nếu đây là topic gốc và user chưa xem
            if (topic.TopicId == null)
            {
                var existingView = await _context.QuestionAnswerTopicViews
                    .AnyAsync(qatv =>
                        qatv.TopicId == id && qatv.UserId == userId.Value &&
                        (qatv.IsDelete == false || qatv.IsDelete == null));
                if (!existingView)
                {
                    var view = new QuestionAnswerTopicView
                    {
                        TopicId = id,
                        QuestionsAnswerId = null,
                        UserId = userId.Value,
                        CreateAt = TimeHelper.NowUsingTimeZone,
                        UpdateAt = TimeHelper.NowUsingTimeZone,
                        UserCreate = userId.Value,
                        UserUpdate = userId.Value,
                        IsDelete = false
                    };
                    _context.QuestionAnswerTopicViews.Add(view);
                    await _context.SaveChangesAsync();

                    if (teachingAssignment.UserId.HasValue)
                    {
                        await _hubContext.Clients.User(teachingAssignment.UserId.Value.ToString())
                            .SendAsync("ReceiveNotification", $"Topic (ID: {id}) đã được xem bởi user {userId.Value}.");
                    }

                    // Cập nhật lại Views sau khi ghi lượt xem
                    topic.Views = await _context.QuestionAnswerTopicViews
                        .CountAsync(qatv => qatv.TopicId == id && (qatv.IsDelete == false || qatv.IsDelete == null));
                }
            }
        }

        return new ApiResponse<TopicResponse>(0, "Lấy topic thành công!", topic);
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
                var parentTopic = await _topicRepository.GetTopicById(request.TopicId.Value);
                if (parentTopic == null)
                {
                    return new ApiResponse<TopicResponse>(1, "Topic cha không tồn tại hoặc đã bị xóa!", null);
                }

                // Lấy thông tin phân công giảng dạy của topic cha
                var teachingAssignment = await _context.TeachingAssignments
                    .FirstOrDefaultAsync(ta => ta.Id == parentTopic.TeachingAssignmentId);
                if (teachingAssignment == null || teachingAssignment.IsDelete == true)
                {
                    return new ApiResponse<TopicResponse>(1, "Phân công giảng dạy của topic cha không tồn tại!", null);
                }

                if (teachingAssignment.ClassId == null)
                {
                    return new ApiResponse<TopicResponse>(1, "Phân công giảng dạy không có lớp học liên kết!", null);
                }

                // Kiểm tra user có thuộc lớp học không (chỉ áp dụng cho học sinh, RoleId = 3)
                var classInfo = await _context.Classes
                    .FirstOrDefaultAsync(c => c.Id == teachingAssignment.ClassId);
                if (classInfo == null)
                {
                    return new ApiResponse<TopicResponse>(1, "Lớp học không tồn tại!", null);
                }

                // Chỉ kiểm tra isUserInClass nếu user không phải giáo viên (RoleId != 2)
                if (user.RoleId != 2)
                {
                    bool isUserInClass =
                        await _topicRepository.IsUserInClassAsync(request.UserId.Value,
                            teachingAssignment.ClassId.Value);
                    if (!isUserInClass)
                    {
                        return new ApiResponse<TopicResponse>(1,
                            $"Bạn không thuộc lớp học {classInfo.Name} để bình luận topic này!", null);
                    }
                }

                // Đảm bảo TeachingAssignmentId của comment khớp với topic cha
                request.TeachingAssignmentId = parentTopic.TeachingAssignmentId ?? 1;
            }

            // 6) Map DTO -> Entity
            var topicEntity = _mapper.Map<Topic>(request);
            topicEntity.CreateAt = TimeHelper.NowUsingTimeZone;
            topicEntity.UpdateAt = TimeHelper.NowUsingTimeZone;
            topicEntity.UserCreate = request.UserId;
            topicEntity.UserUpdate = request.UserId;

            // 7) Upload file nếu có
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

            // 8) Lưu topic/comment vào DB (thông qua repository)
            var savedTopic = await _topicRepository.AddTopic(topicEntity);

            // 9) Map Entity -> Response DTO
            var topicResponse = _mapper.Map<TopicResponse>(savedTopic);

            // 10) Gửi thông báo
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
                                content: $"{user.FullName} đã tạo topic mới '{request.Title}' trong lớp {classInfo?.Name}.",
                                type: false // Thông báo người dùng
                            );
                        }
                    }
                }
            }
            else // Nếu là comment
            {
                var parentTopic = await _topicRepository.GetTopicById(request.TopicId.Value);
                if (parentTopic != null)
                {
                    // Gửi thông báo cho giáo viên (người tạo topic)
                    if (parentTopic.UserId != request.UserId) // Không gửi thông báo cho chính người comment
                    {
                        await _notificationsService.AddNotificationAsync(
                            senderId: request.UserId, // Người comment
                            userId: parentTopic.UserId.Value, // Giáo viên (người tạo topic)
                            subject: "Bình luận mới trong topic của bạn",
                            content: $"{user.FullName} đã bình luận trong topic '{parentTopic.Title}': {request.Description}",
                            type: false // Thông báo người dùng
                        );
                    }
                }
            }

            // 11) Gửi sự kiện SignalR cho các client khác
            string message = request.TopicId.HasValue
                ? "Có comment mới trong topic!"
                : "Có topic mới được tạo!";
            await _hubContext.Clients.AllExcept(new[] { topicResponse.UserId.ToString() })
                .SendAsync("TopicCreated", message, topicResponse);

            // 12) Trả về phản hồi
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
            var existingTopic = await _topicRepository.GetTopicById(request.Id);
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
                    return new ApiResponse<TopicResponse>(1, "Chỉ giáo viên (RoleId = 2) mới được cập nhật topic!", null);
                }
            }

            // 6) Nếu là comment, kiểm tra user có thuộc lớp học liên quan không
            if (existingTopic.TopicId.HasValue)
            {
                // Lấy thông tin topic cha
                var parentTopic = await _topicRepository.GetTopicById(existingTopic.TopicId.Value);
                if (parentTopic == null)
                {
                    return new ApiResponse<TopicResponse>(1, "Topic cha không tồn tại hoặc đã bị xóa!", null);
                }

                // Lấy thông tin phân công giảng dạy của topic cha
                var teachingAssignment = await _context.TeachingAssignments
                    .FirstOrDefaultAsync(ta => ta.Id == parentTopic.TeachingAssignmentId);
                if (teachingAssignment == null || teachingAssignment.IsDelete == true)
                {
                    return new ApiResponse<TopicResponse>(1, "Phân công giảng dạy của topic cha không tồn tại!", null);
                }

                if (teachingAssignment.ClassId == null)
                {
                    return new ApiResponse<TopicResponse>(1, "Phân công giảng dạy không có lớp học liên kết!", null);
                }

                // Kiểm tra user có thuộc lớp học không
                var classInfo = await _context.Classes
                    .FirstOrDefaultAsync(c => c.Id == teachingAssignment.ClassId);
                if (classInfo == null)
                {
                    return new ApiResponse<TopicResponse>(1, "Lớp học không tồn tại!", null);
                }

                // Nếu user không phải giáo viên (RoleId != 2), kiểm tra xem user có thuộc lớp học không
                // và chỉ được cập nhật comment của chính mình
                if (user.RoleId != 2)
                {
                    bool isUserInClass =
                        await _topicRepository.IsUserInClassAsync(request.UserId.Value,
                            teachingAssignment.ClassId.Value);
                    if (!isUserInClass)
                    {
                        return new ApiResponse<TopicResponse>(1,
                            $"Bạn (UserId: {request.UserId}, RoleId: {user.RoleId}) không thuộc lớp học {classInfo.Name} (ClassId: {teachingAssignment.ClassId}) để cập nhật comment này!",
                            null);
                    }

                    // Kiểm tra xem user có phải là người tạo comment không
                    if (existingTopic.UserId != request.UserId)
                    {
                        return new ApiResponse<TopicResponse>(1,
                            "Bạn chỉ được cập nhật comment của chính mình!", null);
                    }
                }

                // Đảm bảo TeachingAssignmentId của comment khớp với topic cha
                request.TeachingAssignmentId = parentTopic.TeachingAssignmentId;
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

            // 9) Gọi repository để cập nhật topic/comment
            var updatedTopic = await _topicRepository.UpdateTopict(topicEntity);
            if (updatedTopic == null)
            {
                return new ApiResponse<TopicResponse>(1, "Topic không tồn tại hoặc đã bị xóa!", null);
            }

            // 10) Map Entity -> Response DTO
            var topicResponse = _mapper.Map<TopicResponse>(updatedTopic);

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
                        if (studentId != request.UserId) // Không gửi thông báo cho chính người cập nhật
                        {
                            await _notificationsService.AddNotificationAsync(
                                senderId: request.UserId, // Người cập nhật topic
                                userId: studentId.Value,
                                subject: "Topic đã được cập nhật!",
                                content: $"{user.FullName} đã cập nhật topic '{request.Title}' trong lớp {classInfo?.Name}.",
                                type: false // Thông báo người dùng
                            );
                        }
                    }
                }
            }
            else // Nếu là comment
            {
                var parentTopic = await _topicRepository.GetTopicById(request.TopicId.Value);
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

            // 12) Gửi sự kiện SignalR cho các client khác
            string message = request.TopicId.HasValue
                ? "Có comment trong topic được cập nhật!"
                : "Có topic được cập nhật!";
            await _hubContext.Clients.AllExcept(new[] { topicResponse.UserId.ToString() })
                .SendAsync("TopicUpdated", message, topicResponse);

            // 13) Trả về phản hồi
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

    public async Task<ApiResponse<bool>> DeleteTopicAsync(int id, int userId)
    {
        try
        {
            // Tìm topic cần xóa
            var topic = await _topicRepository.GetTopicById(id);
            if (topic == null)
            {
                return new ApiResponse<bool>(1, "Topic không tồn tại hoặc đã bị xóa!", false);
            }

            // Lấy thông tin user để kiểm tra vai trò
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ApiResponse<bool>(1, "Không tìm thấy user với Id đã cung cấp!", false);
            }

            // Kiểm tra quyền xóa
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

            // Lấy thông tin phân công giảng dạy
            var teachingAssignment = await _context.TeachingAssignments
                .FirstOrDefaultAsync(ta => ta.Id == topic.TeachingAssignmentId);
            if (teachingAssignment == null || teachingAssignment.IsDelete == true)
            {
                return new ApiResponse<bool>(1, "Phân công giảng dạy không tồn tại!", false);
            }

            if (teachingAssignment.ClassId == null)
            {
                return new ApiResponse<bool>(1, "Phân công giảng dạy không có lớp học liên kết!", false);
            }

            // Xóa topic
            var success = await _topicRepository.DeleteTopic(id);
            if (!success)
            {
                return new ApiResponse<bool>(1, "Xóa topic thất bại!", false);
            }

            // Gửi thông báo
            var classInfo = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == teachingAssignment.ClassId);
            if (!topic.TopicId.HasValue) // Nếu là topic gốc
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
            else // Nếu là comment
            {
                // Gửi thông báo cho người tạo topic cha
                var parentTopic = await _topicRepository.GetTopicById(topic.TopicId.Value);
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

            // Gửi sự kiện SignalR
            await _hubContext.Clients.All.SendAsync("TopicDeleted", id);

            return new ApiResponse<bool>(0, "Xóa topic thành công!", true);
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>(1, $"Có lỗi xảy ra: {ex.Message}", false);
        }
    }

    public async Task<ApiResponse<IEnumerable<TopicResponse>>> SearchTopicsAsync(string? keyword)
    {
        var topics = await _topicRepository.SearchTopic(keyword);
        if (topics == null || !topics.Any())
        {
            return new ApiResponse<IEnumerable<TopicResponse>>(1, "Không tìm thấy topic!", null);
        }

        return new ApiResponse<IEnumerable<TopicResponse>>(0, "Tìm kiếm topic thành công!", topics);
    }

    public async Task<ApiResponse<bool>> SendTopicMessageAsync(int senderId, int receiverId, int topicId,
        string message)
    {
        try
        {
            // Kiểm tra người gửi và người nhận
            var sender = await _context.Users.FindAsync(senderId);
            var receiver = await _context.Users.FindAsync(receiverId);
            if (sender == null || receiver == null)
            {
                return new ApiResponse<bool>(1, "Người gửi hoặc người nhận không tồn tại!", false);
            }

            // Kiểm tra topic
            var topic = await _topicRepository.GetTopicById(topicId);
            if (topic == null)
            {
                return new ApiResponse<bool>(1, "Topic không tồn tại hoặc đã bị xóa!", false);
            }

            // Gửi thông báo người dùng
            await _notificationsService.AddNotificationAsync(
                senderId: senderId, // Người gửi
                userId: receiverId, // Người nhận
                subject: $"Tin nhắn từ {sender.FullName} về topic '{topic.Title}'",
                content: message,
                type: false // Thông báo người dùng
            );

            // Gửi thông báo realtime
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