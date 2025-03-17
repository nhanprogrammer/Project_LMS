using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface IReportService
    {
        Task<AcademicYearReportResponse> GetAcademicYearOverviewAsync(int academicYearId);
        Task<List<ClassPerformanceReport>> GetClassPerformanceReportAsync(int academicYearId, int departmentId);
        Task<SchoolLevelStatisticsResponse> GetSchoolLevelStatisticsAsync(int academicYearId, bool isJuniorHigh);
    }
}