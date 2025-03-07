using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ITopicService
{
    Task<ApiResponse<PaginatedResponse<TopicResponse>>> GetAllTopicsAsync(string? keyword, int pageNumber, int pageSize);
    Task<ApiResponse<TopicResponse>> GetTopicByIdAsync(int id);
    Task<ApiResponse<TopicResponse>> CreateTopicAsync(TopicRequest request);
    Task<ApiResponse<TopicResponse>> UpdateTopicAsync(int id, TopicRequest request);
    Task<ApiResponse<bool>> DeleteTopicAsync(int id);
}
