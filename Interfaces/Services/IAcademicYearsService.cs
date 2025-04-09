using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface IAcademicYearsService
    {
        //Task<IEnumerable<AcademicYearResponse>> GetAllAcademicYears();
        Task<ApiResponse<AcademicYearWithSemestersDto>> GetByIdAcademicYear(int id);
        Task<PaginatedResponse<AcademicYearResponse>> SearchAcademicYear(int year, int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<AcademicYearResponse>> AddAcademicYear(CreateAcademicYearRequest request, int userId);
        Task<ApiResponse<AcademicYearResponse>> UpdateAcademicYear(UpdateAcademicYearRequest request, int userId);
        Task<ApiResponse<AcademicYearResponse>> DeleteLessonAsync(DeleteRequest deleteRequest);

        Task<PaginatedResponse<AcademicYearResponse>> GetPagedAcademicYears(PaginationRequest request, string? keyword);
        Task<List<AcademicYearNameResponse>> GetAcademicYearNamesAsync();
        public Task<ApiResponse<List<AcademicYearNameResponse>>> DropdownAcademicYearsForStudent();
        
    }
}