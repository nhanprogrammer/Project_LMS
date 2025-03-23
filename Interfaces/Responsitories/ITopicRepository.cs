using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface ITopicRepository
{
    Task<IEnumerable<TopicResponse>> GetAllTopic();
    Task<TopicResponse?> GetTopicById(int id);
    Task<Topic> AddTopic(Topic subject);
    Task<Topic?> UpdateTopict(Topic subject);
    Task<bool> DeleteTopic(int id);
    Task<IEnumerable<TopicResponse>> SearchTopic(string? keyword);
    Task<bool> IsUserInClassAsync(int userId, int classId);
}