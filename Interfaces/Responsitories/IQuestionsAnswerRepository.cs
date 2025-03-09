using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface IQuestionsAnswerRepository
{
    Task<IEnumerable<QuestionAnswer>> GetAllAsync();
    Task<QuestionAnswer?> GetByIdAsync(int id);
    Task AddAsync(QuestionAnswer questionsAnswer);
    Task UpdateAsync(QuestionAnswer questionsAnswer);
    Task DeleteAsync(int id);
}