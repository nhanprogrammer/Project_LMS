using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface IQuestionsAnswerRepository
{
    Task<PaginatedResponse<QuestionAnswer>> GetAllAsync(PaginationRequest request);
    Task<QuestionAnswer?> GetByIdAsync(int id);
    Task<QuestionAnswer?> AddAsync(QuestionAnswer questionsAnswer, int teachingAssignmentId);
    Task<QuestionAnswer?> UpdateAsync(QuestionAnswer questionsAnswer, int? newTeachingAssignmentId = null);
    Task<bool> DeleteAsync(int id, int userId);
    Task<IEnumerable<QuestionAnswer>> GetAllQuestionAnswerByTopicIdAsync(int topicId);
    Task<bool> IsUserInClassAsync(int userId, int classId);

    Task<ClassMembersWithStatsResponse> GetClassMembersByTeachingAssignmentAsync(int teachingAssignmentId,
        string? searchTerm = null);

    Task<TeachingAssignmentStudentsResponse> GetTeachingAssignmentStudentsAsync(int teachingAssignmentId);
}