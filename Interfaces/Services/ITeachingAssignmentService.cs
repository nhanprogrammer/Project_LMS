using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ITeachingAssignmentService
{
    Task<TeachingAssignmentResponseCreateUpdate?> GetById(int id);
    Task<TeachingAssignmentResponseCreateUpdate> Create(TeachingAssignmentRequestCreate request);
    Task<TeachingAssignmentResponseCreateUpdate> UpdateById(TeachingAssignmentRequestUpdate request);
    Task<bool> Delete(List<int> ids);
    Task<TeachingAssignmentWrapperResponse> GetTeachingAssignments(int? academicYearId, int? subjectGroupId, int? userId, int pageNumber = 1, int pageSize = 10);
    Task<List<TopicResponseByAssignmentId>> GetTopicsByAssignmentIdAsync(int assignmentId);
    Task<List<ClassResponseSearch>> SearchClass(string? keyword);

}