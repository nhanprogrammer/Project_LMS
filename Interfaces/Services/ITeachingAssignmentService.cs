using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ITeachingAssignmentService
{
    Task<PaginatedResponse<TeachingAssignmentResponse>> GetAll(int pageNumber, int pageSize, int? academicYearId, int? subjectGroupId);
    Task<TeachingAssignmentResponse?> GetById(int id);
    Task<List<TeachingAssignmentResponse>> GetByUserId(int userId);
    Task<TeachingAssignmentResponse> Create(TeachingAssignmentRequestCreate request);
    Task<TeachingAssignmentResponse> UpdateByUserId(int userId, TeachingAssignmentRequest request);
    Task<bool> Delete(List<int> ids);

}