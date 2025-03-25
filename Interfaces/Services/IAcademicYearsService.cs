using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface IAcademicYearsService
    {
        //Task<IEnumerable<AcademicYearResponse>> GetAllAcademicYears();
        Task<AcademicYearResponse> GetByIdAcademicYear(int id);
        Task<ApiResponse<AcademicYearResponse>> AddAcademicYear(CreateAcademicYearRequest request, int userId);
        Task<ApiResponse<AcademicYearResponse>> UpdateAcademicYear(UpdateAcademicYearRequest request, int userId);
        Task<ApiResponse<AcademicYearResponse>> DeleteLessonAsync(DeleteRequest deleteRequest);

        Task<PaginatedResponse<AcademicYearResponse>> GetPagedAcademicYears(PaginationRequest request);
        Task<List<AcademicYearNameResponse>> GetAcademicYearNamesAsync();
        
    }
}