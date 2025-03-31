using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface IStudentTestExamService
{
    Task<ApiResponse<PaginatedResponse<StudentTestExamResponse>>> GetStudentTestExamAsync(int studentId,
        int? pageNumber,
        int? pageSize,
        string? sortDirection
        , string? topicName, string? subjectName, string? department, string? startDate , string? option);

    Task<ApiResponse<List<QuestionResponse>>> GetStudentTestExamByIdAsync(int id , int userId);

    Task<ApiResponse<Object>> SubmitYourAssignment(int userID,SubmitMultipleChoiceQuestionRequest submitMultipleChoiceQuestion);
    Task<ApiResponse<object>> SaveEssay(int UserId, SaveEssayRequest request);
}