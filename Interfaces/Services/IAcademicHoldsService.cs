using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Interfaces
{
    public interface IAcademicHoldsService
    {
        //Task<IEnumerable<AcademicHoldResponse>> GetAllAcademicHold();
        Task<User_AcademicHoldResponse> GetById(int id);
        Task AddAcademicHold(CreateAcademicHoldRequest academicHold);
        Task UpdateAcademicHold(UpdateAcademicHoldRequest academicHold);
        Task<bool> DeleteAcademicHold(int id);
        Task<PaginatedResponse<AcademicHoldResponse>> GetPagedAcademicHolds(PaginationRequest request);
        Task<List<Class_UserResponse>> GetAllUser_Class();
        Task<List<User_AcademicHoldsResponse>> GetAllUserName();



    }
}