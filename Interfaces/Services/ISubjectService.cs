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
        Task<ApiResponse<SubjectResponse>> UpdateSubjectAsync(int id, SubjectRequest request);
        Task<ApiResponse<bool>> DeleteSubjectAsync(int id);
    }
}