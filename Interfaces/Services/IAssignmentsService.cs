using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface IAssignmentsService
    {
        Task<AssignmentsResponse> GetAssignmentById(int id);
        Task<IEnumerable<AssignmentsResponse>> GetAllAssignments();
        Task AddAssignment(CreateAssignmentRequest request);
        Task UpdateAssignment(int id, UpdateAssignmentRequest request);
        Task<bool> DeleteAssignment(int id);
    }
}