using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface IAcademicHoldsService
    {
        //Task<IEnumerable<AcademicHoldResponse>> GetAllAcademicHold();
        Task<AcademicHoldResponse> GetByIdAcademicHold(int id);
        Task AddAcademicHold(CreateAcademicHoldRequest academicHold);
        Task UpdateAcademicHold(UpdateAcademicHoldRequest academicHold);
        Task<bool> DeleteAcademicHold(int id);
        Task<PaginatedResponse<AcademicHoldResponse>> GetPagedAcademicHolds(PaginationRequest request);

    }
}