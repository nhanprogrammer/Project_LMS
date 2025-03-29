using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ITopicService
{
    Task<ApiResponse<IEnumerable<TopicResponse>>> GetAllTopicsAsync(int userId, int teachingAssignmentId);
    Task<ApiResponse<TopicResponse>> GetTopicByIdAsync(int userId, int teachingAssignmentId, int id);
    Task<ApiResponse<TopicResponse>> CreateTopicAsync(CreateTopicRequest request);
    Task<ApiResponse<TopicResponse>> UpdateTopicAsync(UpdateTopicRequest request);
    Task<ApiResponse<bool>> DeleteTopicAsync(int userId, int teachingAssignmentId, int id);

    Task<ApiResponse<IEnumerable<TopicResponse>>> SearchTopicsAsync(int userId, int teachingAssignmentId,
        string? keyword);

    Task<ApiResponse<bool>> SendTopicMessageAsync(int senderId, int teachingAssignmentId, int receiverId,
        int topicId, string message);
}