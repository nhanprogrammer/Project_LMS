using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ITeacherTestExamService
{
    Task<ApiResponse<PaginatedResponse<TeacherTestExamResponse>>> GetTeacherTestExamAsync( int userId,int? pageNumber,
        int? pageSize,
        string? sortDirection
        ,string? topicName, int? subjectId, int? departmentId,string? startDate, string? tab);
    Task<ApiResponse<object?>> CreateTeacherTestExamAsync(int userId,TeacherTestExamRequest request);
    Task<ApiResponse<object?>> UpdateTeacherTestExamAsync(int userId,TeacherTestExamRequest request);
    
    Task<ApiResponse<object?>> GetTeacherTestExamById(int id , int classId);
    Task<ApiResponse<object?>> GetFilterClass(int departmentId);
    
    Task<ApiResponse<object?>> DeleteTeacherTestExamById(int id);
    Task<ApiResponse<object?>> StarTeacherTestExamById(StartTestExamRequest request);
    
    
}