using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface IAcademicYearsService
    {
        //Task<IEnumerable<AcademicYearResponse>> GetAllAcademicYears();
        Task<AcademicYearResponse> GetByIdAcademicYear(int id);
        Task AddAcademicYear(CreateAcademicYearRequest request);
        Task UpdateAcademicYear(int id, UpdateAcademicYearRequest request);
        Task<bool> DeleteAcademicYear(int id);

        Task<PaginatedResponse<AcademicYearResponse>> GetPagedAcademicYears(PaginationRequest request);
    }
}