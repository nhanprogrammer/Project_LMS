using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface IDepartmentsService
    {
        Task<ApiResponse<List<DepartmentResponse>>> GetAllCoursesAsync();
        Task<ApiResponse<DepartmentResponse>> CreateDepartmentAsync(CreateDepartmentRequest createDepartmentRequest);
        Task<ApiResponse<DepartmentResponse>> UpdateDepartmentAsync(int id, UpdateDepartmentRequest updateDepartmentRequest);
        Task<ApiResponse<DepartmentResponse>> DeleteDepartmentAsync(string id);
    }
}