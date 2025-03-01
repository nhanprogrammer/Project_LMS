using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ITopicService
{
    
    Task<ApiResponse<List<TopicResponse>>> GetAll();
    Task<ApiResponse<TopicResponse>> Create(TopicRequest request);
    Task<ApiResponse<TopicResponse>> Update(int id, TopicRequest request);
    Task<ApiResponse<TopicResponse>> Delete(int id);
    Task<ApiResponse<TopicResponse>> Search(int id);
    
}