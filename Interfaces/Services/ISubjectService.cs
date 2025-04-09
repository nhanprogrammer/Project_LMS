using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using System.Threading.Tasks;

namespace Project_LMS.Interfaces.Services
{
    public interface ISubjectService
    {
        Task<ApiResponse<PaginatedResponse<SubjectResponse>>> GetAllSubjectsAsync(string? keyword, int pageNumber, int pageSize);
        Task<ApiResponse<SubjectResponse>> GetSubjectByIdAsync(int id);
        Task<ApiResponse<SubjectResponse>> CreateSubjectAsync(SubjectRequest request);
        Task<ApiResponse<SubjectResponse>> UpdateSubjectAsync(SubjectRequest request);
        // Task<ApiResponse<bool>> DeleteSubjectAsync(int id);
        Task<ApiResponse<bool>> DeleteMultipleSubjectsAsync(List<int> ids);

        Task<List<SubjectResponseSearch>> getSubjectByUserId(int userId);
        Task<List<SubjectDropdownResponse>> GetSubjectsBySubjectGroupIdAsync(int subjectGroupId);
        Task<List<SubjectDropdownResponse>> GetSubjectDropdownAsync();
        Task<List<SubjectDropdownResponse>> GetSubjectDropdownBySubjectGroupIdAsync(int subjectGroupId);
        Task<List<SubjectDropdownResponse>> GetSubjectDropdownByStudentAsync();
    }
}