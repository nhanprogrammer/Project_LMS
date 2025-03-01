using Project_LMS.Models;

namespace Project_LMS.Interfaces.Repositories
{
    public interface IQuestionsAnswerTopicViewRepository
    {
        Task<IEnumerable<QuestionsAnswerTopicView>> GetAllAsync();
        Task<QuestionsAnswerTopicView?> GetByIdAsync(int id);
        Task AddAsync(QuestionsAnswerTopicView questionsAnswerTopicView);
        Task UpdateAsync(QuestionsAnswerTopicView questionsAnswerTopicView);
        Task DeleteAsync(int id);
    }
}