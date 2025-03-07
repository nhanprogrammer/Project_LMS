using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface ISubjectTypeService
    {
        Task<ApiResponse<PaginatedResponse<SubjectTypeResponse>>> GetAllSubjectTypesAsync(int pageNumber, int pageSize);
        Task<ApiResponse<SubjectTypeResponse>> GetSubjectTypeByIdAsync(int id);
        Task<ApiResponse<SubjectTypeResponse>> CreateSubjectTypeAsync(SubjectTypeRequest request);
        Task<ApiResponse<SubjectTypeResponse>> UpdateSubjectTypeAsync(int id, SubjectTypeRequest request);
        Task<ApiResponse<bool>> DeleteSubjectTypeAsync(int id);
    }
}