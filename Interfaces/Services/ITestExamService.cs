using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ITestExamService
{
    Task<ApiResponse<PaginatedResponse<TestExamResponse>>> GetAllTestExamsAsync(string? keyword, int pageNumber,
        int pageSize);

    Task<ApiResponse<TestExamResponse>> GetTestExamByIdAsync(int id);
    Task<ApiResponse<object>> CreateTestExamAsync(CreateTestExamRequest request);
    Task<ApiResponse<object>> UpdateTestExamAsync(int id, UpdateTestExamRequest request);
    Task<ApiResponse<bool>> DeleteTestExamAsync(int id);
    Task<ApiResponse<IEnumerable<object>>> FilterClasses(int academicYearId, int departmentId);
    Task<ApiResponse<IEnumerable<object>>> GetAllAcademicYear();
    Task<ApiResponse<IEnumerable<object>>> GetAllClasses();
    Task<ApiResponse<IEnumerable<object>>> GetAllAssignmentOfMarking();
}