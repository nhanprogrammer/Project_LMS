using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public interface IReportRepository
    {
        Task<AcademicYear?> GetAcademicYearByIdAsync(int academicYearId);
        Task<List<ClassPerformanceReport>> GetClassPerformanceReportAsync(int academicYearId, int departmentId);
        Task<int> GetTotalStudentsAsync(int academicYearId);
        Task<int> GetTotalTeachersAsync(int academicYearId);
        Task<int> GetTotalClassesAsync(int academicYearId);

    }
}
