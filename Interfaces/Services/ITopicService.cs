using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ITopicService
{
    Task<ApiResponse<IEnumerable<TopicResponse>>> GetAllTopicsAsync();
    Task<ApiResponse<TopicResponse>> GetTopicByIdAsync(int id, int? userId);
    Task<ApiResponse<TopicResponse>> CreateTopicAsync(CreateTopicRequest request);
    Task<ApiResponse<TopicResponse>> UpdateTopicAsync(UpdateTopicRequest request);
    Task<ApiResponse<bool>> DeleteTopicAsync(int id, int userId);
    Task<ApiResponse<IEnumerable<TopicResponse>>> SearchTopicsAsync(string? keyword);
    Task<ApiResponse<bool>> SendTopicMessageAsync(int senderId, int receiverId, int topicId, string message);
}