using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ITeachingAssignmentService
{
    Task<PaginatedResponse<TeachingAssignmentResponseCreateUpdate>> GetAll(int pageNumber, int pageSize, int? academicYearId, int? subjectGroupId);
    Task<TeachingAssignmentResponseCreateUpdate?> GetById(int id);
    //Task<List<TeachingAssignmentResponse>> GetByUserId(int userId);
    Task<TeachingAssignmentResponseCreateUpdate> Create(TeachingAssignmentRequestCreate request);
    Task<TeachingAssignmentResponseCreateUpdate> UpdateByUserId(int userId, TeachingAssignmentRequest request);
    Task<bool> Delete(List<int> ids);
    Task<TeachingAssignmentWrapperResponse> GetTeachingAssignments(int? academicYearId, int? subjectGroupId, int? userId, int pageNumber = 1, int pageSize = 10);
    Task<List<TopicResponseByAssignmentId>> GetTopicsByAssignmentIdAsync(int assignmentId);
}