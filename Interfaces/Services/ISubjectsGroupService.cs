using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface ISubjectsGroupService
    {
        Task<ApiResponse<PaginatedResponse<SubjectsGroupResponse>>> GetAllSubjectsGroupsAsync(int pageNumber, int pageSize);
        Task<ApiResponse<SubjectsGroupResponse>> GetSubjectsGroupByIdAsync(int id);
        Task<ApiResponse<SubjectsGroupResponse>> CreateSubjectsGroupAsync(SubjectsGroupRequest request);
        Task<ApiResponse<SubjectsGroupResponse>> UpdateSubjectsGroupAsync(int id, SubjectsGroupRequest request);
        Task<ApiResponse<bool>> DeleteSubjectsGroupAsync(int id);
    }
}