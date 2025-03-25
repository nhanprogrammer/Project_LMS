using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Interfaces
{
    public interface IAcademicHoldsService
    {
        //Task<IEnumerable<AcademicHoldResponse>> GetAllAcademicHold();
        Task<User_AcademicHoldResponse> GetById(int id);
        Task<AcademicHoldResponse> AddAcademicHold(CreateAcademicHoldRequest academicHold, int userId);
        Task<AcademicHoldResponse> UpdateAcademicHold(UpdateAcademicHoldRequest academicHold, int userId);
        Task<bool> DeleteAcademicHold(int id);
        Task<PaginatedResponse<AcademicHoldResponse>> GetPagedAcademicHolds(PaginationRequest request);
        Task<List<User_AcademicHoldsResponse>> GetAllUserName();
        Task<SemesterResponse?> GetSemesterByDateAsync(string dateString);
        Task<List<User_AcademicHoldsResponse>> SearchUsersByCriteriaAsync(int classId, string keyword);
    }
}