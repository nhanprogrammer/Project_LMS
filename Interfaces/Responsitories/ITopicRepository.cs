using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface ITopicRepository
{
    Task<PaginatedResponse<Topic>> GetAllTopic(int pageNumber, int pageSize);
    Task<Topic?> GetTopicById(int id);
    Task<Topic> AddTopic(Topic subject);
    Task<Topic?> UpdateTopict(Topic subject);
    Task<bool> DeleteTopic(int id);
    Task<Topic?> SearchTopic(string? keyword);
}