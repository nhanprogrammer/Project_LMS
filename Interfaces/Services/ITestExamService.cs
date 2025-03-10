using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ITestExamService
{
    Task<ApiResponse<PaginatedResponse<TestExamResponse>>> GetAllTestExamsAsync(string? keyword, int pageNumber, int pageSize);
    Task<ApiResponse<TestExamResponse>> GetTestExamByIdAsync(int id);
    Task<ApiResponse<TestExamResponse>> CreateTestExamAsync(TestExamRequest request);
    Task<ApiResponse<TestExamResponse>> UpdateTestExamAsync(int id, TestExamRequest request);
    Task<ApiResponse<bool>> DeleteTestExamAsync(int id);
    
}