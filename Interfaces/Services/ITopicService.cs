using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ITopicService
{
    Task<ApiResponse<PaginatedResponse<TopicResponse>>> GetAllTopicsAsync(int pageNumber, int pageSize);
    Task<ApiResponse<TopicResponse>> GetTopicByIdAsync(int id);
    Task<ApiResponse<TopicResponse>> CreateTopicAsync(CreateTopicRequest request);
    Task<ApiResponse<TopicResponse>> UpdateTopicAsync(UpdateTopicRequest request);
    Task<ApiResponse<bool>> DeleteTopicAsync(int id);
    Task<ApiResponse<IEnumerable<TopicResponse>>> SearchTopicsAsync(string? keyword);
}