using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface IReportService
    {
        Task<AcademicYearReportResponse> GetAcademicYearOverviewAsync(int academicYearId);
        Task<List<ClassPerformanceReport>> GetClassPerformanceReportAsync(int academicYearId, int departmentId);
        Task<SchoolLevelStatisticsResponse> GetSchoolLevelStatisticsAsync(int academicYearId, bool isJuniorHigh);

        //Thống kê teacher
        Task<TeacherStatisticsResponse> GetTeacherStatisticsAsync(int userId);
        Task<TeacherPerformanceReport> GetTeacherPerformanceReportAsync(int userId);
        Task<List<TeacherSemesterStatisticsResponse>> GetTeacherSemesterStatisticsAsync(int teacherId);
    }
}