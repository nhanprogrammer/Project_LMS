using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ITeachingAssignmentService
{
    Task<PaginatedResponse<TeachingAssignmentResponse>> GetAll(int pageNumber, int pageSize);
    Task<TeachingAssignmentResponse?> GetById(int id);
    Task<TeachingAssignmentResponse> Create(TeachingAssignmentRequest request);
    Task<bool> Update(int id, TeachingAssignmentRequest request);
    Task<bool> Delete(int id);
}