using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ISubjectGroupService
{
    Task<ApiResponse<SubjectGroupResponse>> GetSubjectGroupById(int id);
    Task<ApiResponse<PaginatedResponse<SubjectGroupResponse>>> GetAllSubjectGroupAsync(    int? pageNumber,
        int? pageSize,
        string? sortDirection);
    Task<ApiResponse<SubjectGroupResponse>> CreateSubjectGroupAsync(CreateSubjectGroupRequest createSubjectGroupRequest);
    Task<ApiResponse<SubjectGroupResponse>> UpdateSubjectGroupAsync(int id,  UpdateSubjectGroupRequest updateSubjectGroupRequest);
    Task<ApiResponse<SubjectGroupResponse>> DeleteSubjectGroupAsync(int id);
    
    Task<ApiResponse<SubjectGroupResponse>> DeleteSubjectGroupSubject(List<int> ids);
}