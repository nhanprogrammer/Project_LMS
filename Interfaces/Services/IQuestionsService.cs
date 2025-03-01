using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface IQuestionsService
    {
        Task<IEnumerable<QuestionResponse>> GetAllAsync();
        Task<QuestionResponse> GetByIdAsync(int id);
        Task<QuestionResponse> CreateAsync(QuestionRequest request);
        Task<QuestionResponse> UpdateAsync(int id, QuestionRequest request);
        Task<QuestionResponse> DeleteAsync(int id);
    }
}