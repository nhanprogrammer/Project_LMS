using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.SignalR;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Helpers;
using Project_LMS.Hubs;
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

    public TopicService(ITopicRepository topicRepository, IMapper mapper, ICloudinaryService cloudinary,
        IHubContext<RealtimeHub> hubContext, ITeachingAssignmentService teachingAssignmentService,
        IUserRepository userRepository)
    {
        _topicRepository = topicRepository;
        _mapper = mapper;
        _cloudinary = cloudinary;
        _hubContext = hubContext;
        _teachingAssignmentRepository = teachingAssignmentService;
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<PaginatedResponse<TopicResponse>>> GetAllTopicsAsync(int pageNumber, int pageSize)
    {
        // 1) Gọi repo lấy PaginatedResponse<Topic
        var response = await _topicRepository.GetAllTopic(pageNumber, pageSize);
        // 2) Map sang PaginatedResponse<TopicResponse>
        var mappedResponse = _mapper.Map<PaginatedResponse<TopicResponse>>(response);
        return new ApiResponse<PaginatedResponse<TopicResponse>>(0, "Lấy danh sách topic thành công!", mappedResponse);
    }

    public async Task<ApiResponse<TopicResponse>> GetTopicByIdAsync(int id)
    {
        var topic = await _topicRepository.GetTopicById(id);
        if (topic == null)
        {
            return new ApiResponse<TopicResponse>(1, "Không tìm thấy topic!", null);
        }

        var mappedResponse = _mapper.Map<TopicResponse>(topic);
        return new ApiResponse<TopicResponse>(0, "Lấy topic thành công!", mappedResponse);
    }

    public async Task<ApiResponse<TopicResponse>> CreateTopicAsync(CreateTopicRequest request)
    {
        try
        {
            // 1) Kiểm tra thông tin người dùng
            var user = await _userRepository.FindAsync(request.UserId ?? 1);
            if (user == null)
            {
                return new ApiResponse<TopicResponse>(1, "User không tồn tại!", null);
            }

            if (user.RoleId != 2)
            {
                return new ApiResponse<TopicResponse>(1, "Chỉ giáo viên mới được tạo topic.", null);
            }

            // 2) Kiểm tra phân công giảng dạy
            var teachingAssignment = await _teachingAssignmentRepository.GetById(request.TeachingAssignmentId);
            if (teachingAssignment == null)
            {
                return new ApiResponse<TopicResponse>(1, "Phân công giảng dạy không tồn tại!", null);
            }

            if (teachingAssignment.UserId != user.Id)
            {
                return new ApiResponse<TopicResponse>(1,
                    $"Giáo viên không thuộc lớp học {teachingAssignment.ClassName} này để giảng dạy!", null);
            }

            // 3) Map DTO -> Entity
            var topicEntity = _mapper.Map<Topic>(request);

            // 4) Upload file nếu có
            if (!string.IsNullOrEmpty(request.FileName))
            {
                topicEntity.FileName = await _cloudinary.UploadImageAsync(request.FileName);
            }

            topicEntity.CreateAt = TimeHelper.NowUsingTimeZone;
            topicEntity.IsDelete = false;

            // 5) Lưu topic vào DB (thông qua repository)
            var savedTopic = await _topicRepository.AddTopic(topicEntity);

            // 6) Map Entity -> Response DTO
            var topicResponse = _mapper.Map<TopicResponse>(savedTopic);

            // 7) Gửi sự kiện SignalR cho các client khác (ngoại trừ người tạo)
            await _hubContext.Clients.AllExcept(new[] { topicResponse.UserId.ToString() })
                .SendAsync("TopicCreated", topicResponse);

            return new ApiResponse<TopicResponse>(0, "Thêm topic thành công!", topicResponse);
        }
        catch (Exception ex)
        {
            // Log exception nếu cần
            return new ApiResponse<TopicResponse>(1, ex.Message, null);
        }
    }


    public async Task<ApiResponse<TopicResponse>> UpdateTopicAsync(UpdateTopicRequest request)
    {
        try
        {
            // 1) Tìm topic cũ
            var existingTopic = await _topicRepository.GetTopicById(request.Id);
            if (existingTopic == null)
            {
                return new ApiResponse<TopicResponse>(1, "Topic không tồn tại", null);
            }

            // 2) Kiểm tra thông tin người dùng (nếu UpdateTopicRequest có UserId, nếu không bạn có thể sử dụng existingTopic.UserId)
            var user = await _userRepository.FindAsync(request.UserId ?? existingTopic.UserId ?? 1);
            if (user == null)
            {
                return new ApiResponse<TopicResponse>(1, "User không tồn tại!", null);
            }

            if (user.RoleId != 2)
            {
                return new ApiResponse<TopicResponse>(1, "Chỉ giáo viên mới được cập nhật topic.", null);
            }

            // 3) Kiểm tra phân công giảng dạy của topic hiện có
            var teachingAssignment =
                await _teachingAssignmentRepository.GetById(existingTopic.TeachingAssignmentId ?? 1);
            if (teachingAssignment == null)
            {
                return new ApiResponse<TopicResponse>(1, "Phân công giảng dạy không tồn tại!", null);
            }

            if (teachingAssignment.UserId != user.Id)
            {
                return new ApiResponse<TopicResponse>(1,
                    $"Giáo viên không thuộc lớp học {teachingAssignment.ClassName} này để cập nhật topic!", null);
            }

            // 4) Cập nhật các trường nếu có trong request
            existingTopic.Title = !string.IsNullOrEmpty(request.Title) ? request.Title : existingTopic.Title;
            existingTopic.Description = !string.IsNullOrEmpty(request.Description)
                ? request.Description
                : existingTopic.Description;

            // 5) Nếu có ảnh mới thì upload lên Cloudinary
            if (!string.IsNullOrEmpty(request.FileName))
            {
                existingTopic.FileName = await _cloudinary.UploadImageAsync(request.FileName);
            }

            // 6) Gọi repository update
            var updatedTopic = await _topicRepository.UpdateTopict(existingTopic);
            var topicResponse = _mapper.Map<TopicResponse>(updatedTopic);

            // 7) Gửi sự kiện SignalR cho các client khác (ngoại trừ người tạo)
            await _hubContext.Clients.AllExcept(new[] { topicResponse.UserId.ToString() })
                .SendAsync("TopicUpdated", topicResponse);

            return new ApiResponse<TopicResponse>(0, "Cập nhật topic thành công!", topicResponse);
        }
        catch (Exception ex)
        {
            // Ghi log lỗi nếu cần
            return new ApiResponse<TopicResponse>(1, ex.Message, null);
        }
    }


    public async Task<ApiResponse<bool>> DeleteTopicAsync(int id)
    {
        var success = await _topicRepository.DeleteTopic(id);
        if (!success)
        {
            return new ApiResponse<bool>(1, "Topic không tồn tại", false);
        }

        await _hubContext.Clients.All.SendAsync("TopicDeleted", id);

        return new ApiResponse<bool>(0, "Xóa topic thành công!", true);
    }

    public async Task<ApiResponse<IEnumerable<TopicResponse>>> SearchTopicsAsync(string? keyword)
    {
        var topics = await _topicRepository.SearchTopic(keyword);
        if (topics == null || !topics.Any())
        {
            return new ApiResponse<IEnumerable<TopicResponse>>(1, "Không tìm thấy topic!", null);
        }

        return new ApiResponse<IEnumerable<TopicResponse>>(0, "Tìm kiếm topic thành công!",
            _mapper.Map<IEnumerable<TopicResponse>>(topics));
    }
}