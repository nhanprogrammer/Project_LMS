using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface IAcademicYearsService
    {
        //Task<IEnumerable<AcademicYearResponse>> GetAllAcademicYears();
        Task<AcademicYearResponse> GetByIdAcademicYear(int id);
        Task AddAcademicYear(CreateAcademicYearRequest request);
        Task<ApiResponse<AcademicYearResponse>> UpdateAcademicYear(UpdateAcademicYearRequest request);
        Task<ApiResponse<AcademicYearResponse>> DeleteLessonAsync(DeleteRequest deleteRequest);

        Task<PaginatedResponse<AcademicYearResponse>> GetPagedAcademicYears(PaginationRequest request);
        
    }
}