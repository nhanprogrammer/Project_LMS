using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface IQuestionsAnswerRepository
{
    Task<IEnumerable<QuestionsAnswer>> GetAllAsync();
    Task<QuestionsAnswer?> GetByIdAsync(int id);
    Task AddAsync(QuestionsAnswer questionsAnswer);
    Task UpdateAsync(QuestionsAnswer questionsAnswer);
    Task DeleteAsync(int id);
}