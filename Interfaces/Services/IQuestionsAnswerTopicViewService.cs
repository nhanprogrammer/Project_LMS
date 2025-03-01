using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface IQuestionsAnswerTopicViewService
    {
        Task<IEnumerable<QuestionsAnswerTopicViewResponse>> GetAllAsync();
        Task<QuestionsAnswerTopicViewResponse> GetByIdAsync(int id);
        Task<QuestionsAnswerTopicViewResponse> CreateAsync(QuestionsAnswerTopicViewRequest request);
        Task<QuestionsAnswerTopicViewResponse> UpdateAsync(int id, QuestionsAnswerTopicViewRequest request);
        Task<QuestionsAnswerTopicViewResponse> DeleteAsync(int id);
    }
}