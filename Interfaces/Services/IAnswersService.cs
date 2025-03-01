using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface IAnswersService
    {
        Task<IEnumerable<AnswerResponse>> GetAllAnswers();
        Task<AnswerResponse?> GetAnswerById(int id);
        Task AddAnswer(CreateAnswerRequest request);
        Task UpdateAnswer(int id, UpdateAnswerRequest request);
        Task<bool> DeleteAnswer(int id);
    }
}