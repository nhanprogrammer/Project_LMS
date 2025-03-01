using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface IAssignmentDetailsService
    {
        Task<AssignmentDetailResponse> GetAssignmentDetailById(int id);
        Task<IEnumerable<AssignmentDetailResponse>> GetAllAssignmentDetails();
        Task AddAssignmentDetail(CreateAssignmentDetailRequest request);
        Task UpdateAssignmentDetail(int id, UpdateAssignmentDetailRequest request);
        Task<bool> DeleteAssignmentDetail(int id);
    }
}