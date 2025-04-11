using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface ITopicRepository
{
    Task<IEnumerable<TopicResponse>> GetAllTopic(int userId, int teachingAssignmentId);
    Task<TopicResponse?> GetTopicById(int userId, int teachingAssignmentId, int id);
    Task<Topic> AddTopic(Topic subject);
    Task<Topic?> UpdateTopic(Topic subject);
    Task<bool> DeleteTopic(int userId, int teachingAssignmentId, int id);
    Task<IEnumerable<TopicResponse>> SearchTopic(int userId, int teachingAssignmentId, string? keyword);
    Task<bool> IsUserInClassAsync(int userId, int classId);
    Task<int> CountByAssignmentIdAsync(int teachingAssignmentId);
     Task<List<Topic>> GetByAssignmentIdPaginatedAsync(
        int teachingAssignmentId,
        int pageNumber,
        int pageSize,
        string? column = null,
        bool orderBy = true,
        string? searchItem = null);
}