using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface IQuestionsAnswerRepository
{
    Task<PaginatedResponse<QuestionAnswer>> GetAllAsync(PaginationRequest request);
    Task<QuestionAnswer?> GetByIdAsync(int id);
    Task<QuestionAnswer?> AddAsync(QuestionAnswer questionsAnswer, int topicId, int userId);
    Task<QuestionAnswer?> UpdateAsync(QuestionAnswer questionsAnswer, int? newTopicId = null);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable <QuestionAnswer>> GetAllQuestionAnswerByTopicIdAsync(int topicId);
}