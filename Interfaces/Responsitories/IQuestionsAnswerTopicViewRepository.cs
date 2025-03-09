using Project_LMS.Models;

namespace Project_LMS.Interfaces.Repositories
{
    public interface IQuestionsAnswerTopicViewRepository
    {
        Task<IEnumerable<QuestionAnswerTopicView>> GetAllAsync();
        Task<QuestionAnswerTopicView?> GetByIdAsync(int id);
        Task AddAsync(QuestionAnswerTopicView questionsAnswerTopicView);
        Task UpdateAsync(QuestionAnswerTopicView questionsAnswerTopicView);
        Task DeleteAsync(int id);
    }
}