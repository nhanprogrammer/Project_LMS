using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.SignalR;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
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
    private static readonly object _lock = new object();

    public TopicService(ITopicRepository topicRepository, IMapper mapper, ICloudinaryService cloudinary,
        IHubContext<RealtimeHub> hubContext)
    {
        _topicRepository = topicRepository;
        _mapper = mapper;
        _cloudinary = cloudinary;
        _hubContext = hubContext;
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
        // 1) Map DTO -> Entity
        var topicResquest = _mapper.Map<Topic>(request);

        // 2) Upload file (nếu có)
        if (!string.IsNullOrEmpty(request.FileName))
        {
            topicResquest.FileName = await _cloudinary.UploadImageAsync(request.FileName);
        }

        // 3) Lưu vào DB
        var savedTopic = await _topicRepository.AddTopic(topicResquest);

        // 4) Map Entity -> DTO
        var topicResponse = _mapper.Map<TopicResponse>(savedTopic);

        // 5) Gửi sự kiện SignalR
        lock (_lock)
        {
            _ = Task.Run(() => _hubContext.Clients.All.SendAsync("TopicCreated", topicResponse));
        }

        return new ApiResponse<TopicResponse>(0, "Thêm topic thành công!", topicResponse);
    }

    public async Task<ApiResponse<TopicResponse>> UpdateTopicAsync(UpdateTopicRequest request)
    {
        // 1) Tìm topic cũ
        var existingTopic = await _topicRepository.GetTopicById(request.Id);

        if (existingTopic == null)
        {
            return new ApiResponse<TopicResponse>(1, "Topic không tồn tại", null);
        }

        // 2) Cập nhật từng trường nếu có trong request
        existingTopic.Title = !string.IsNullOrEmpty(request.Title) ? request.Title : existingTopic.Title;
        existingTopic.Description =
            !string.IsNullOrEmpty(request.Description) ? request.Description : existingTopic.Description;

        // 3) Nếu có ảnh mới thì upload lên Cloudinary
        if (!string.IsNullOrEmpty(request.FileName))
        {
            existingTopic.FileName = await _cloudinary.UploadImageAsync(request.FileName);
        }

        // 4) Gọi repo update
        var updatedTopic = await _topicRepository.UpdateTopict(existingTopic);
        var topicResponse = _mapper.Map<TopicResponse>(updatedTopic);

        // 5) Realtime SignalR
        await _hubContext.Clients.All.SendAsync("TopicUpdated", topicResponse);

        return new ApiResponse<TopicResponse>(0, "Cập nhật topic thành công!", topicResponse);
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
        if (topics == null)
        {
            return new ApiResponse<IEnumerable<TopicResponse>>(1, "Không tìm thấy topic!", null);
        }

        return new ApiResponse<IEnumerable<TopicResponse>>(0, "Tìm kiếm topic thành công!",
            _mapper.Map<IEnumerable<TopicResponse>>(topics));
    }
}