using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface IClassTypeService
    {
        Task<ApiResponse<PaginatedResponse<ClassTypeResponse>>> GetAllClassTypesAsync(string? keyword, int pageNumber, int pageSize);
        Task<ApiResponse<ClassTypeResponse>> GetClassTypeByIdAsync(int id);
        Task<ApiResponse<ClassTypeResponse>> CreateClassTypeAsync(ClassTypeRequest request);
        Task<ApiResponse<ClassTypeResponse>> UpdateClassTypeAsync(int id, ClassTypeRequest request);
        Task<ApiResponse<bool>> DeleteClassTypeAsync(int id);
    }
}