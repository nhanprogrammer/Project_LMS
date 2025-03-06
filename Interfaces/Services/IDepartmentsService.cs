using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface IDepartmentsService
    {
        Task<ApiResponse<PaginatedResponse<DepartmentResponse>>> GetAllCoursesAsync(
            int? pageNumber, 
            int? pageSize,
            string? sortDirection
            );

        Task<ApiResponse<DepartmentResponse>> CreateDepartmentAsync(CreateDepartmentRequest createDepartmentRequest);

        Task<ApiResponse<DepartmentResponse>> UpdateDepartmentAsync(int id,
            UpdateDepartmentRequest updateDepartmentRequest);

        Task<ApiResponse<DepartmentResponse>> DeleteDepartmentAsync(string id);
        Task<ApiResponse<IEnumerable<DepartmentResponse>>> SearchDepartmentsAsync(string? keyword);
        Task<ApiResponse<IEnumerable<object>>> GetAllClassesAsync(int DeparmentId);
        Task<ApiResponse<string>> DeleteClassById(List<int> classId);

    }
}