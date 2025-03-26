using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ITeacherTestExamService
{
    Task<ApiResponse<PaginatedResponse<TeacherTestExamResponse>>> GetTeacherTestExamAsync(int? pageNumber,
        int? pageSize,
        string? sortDirection
        ,string? topicName, string? subjectName, string? department,string? startDate);
    Task<ApiResponse<object?>> CreateTeacherTestExamAsync(TeacherTestExamRequest request);
    Task<ApiResponse<object?>> UpdateTeacherTestExamAsync(TeacherTestExamRequest request);
    
    Task<ApiResponse<object?>> GetTeacherTestExamById(int id);
    Task<ApiResponse<object?>> GetFilterClass(int departmentId);
    
    Task<ApiResponse<object?>> DeleteTeacherTestExamById(int id);
}