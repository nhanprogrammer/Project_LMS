using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Repositories;

namespace Project_LMS.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<AcademicYearReportResponse> GetAcademicYearOverviewAsync(int academicYearId)
        {
            var academicYear = await _reportRepository.GetAcademicYearByIdAsync(academicYearId);
            if (academicYear == null)
            {
                throw new NotFoundException("Không tìm thấy niên khóa.");
            }

            var totalStudents = await _reportRepository.GetTotalStudentsAsync(academicYearId);
            var totalTeachers = await _reportRepository.GetTotalTeachersAsync(academicYearId);
            var totalClasses = await _reportRepository.GetTotalClassesAsync(academicYearId);

            return new AcademicYearReportResponse
            {
                TotalStudents = totalStudents,
                TotalTeachers = totalTeachers,
                TotalClasses = totalClasses
            };
        }

        public async Task<List<ClassPerformanceReport>> GetClassPerformanceReportAsync(int academicYearId, int departmentId)
        {
            return await _reportRepository.GetClassPerformanceReportAsync(academicYearId, departmentId);
        }

        public async Task<SchoolLevelStatisticsResponse> GetSchoolLevelStatisticsAsync(int academicYearId, bool isJuniorHigh)
        {
            var statistics = await _reportRepository.GetSchoolLevelStatisticsAsync(academicYearId, isJuniorHigh);
            return statistics;
        }

        //Thống kê teacher
        public async Task<TeacherStatisticsResponse> GetTeacherStatisticsAsync(int userId)
        {
            var totalClasses = await _reportRepository.GetTotalClassesByTeacherAsync(userId);
            var totalOnlineClasses = await _reportRepository.GetTotalOnlineClassesByTeacherAsync(userId);
            var totalUngradedAssignments = await _reportRepository.GetTotalUngradedAssignmentsByTeacherAsync(userId);
            var totalQuestionsReceived = await _reportRepository.GetTotalQuestionsReceivedByTeacherAsync(userId);

            return new TeacherStatisticsResponse
            {
                TotalClasses = totalClasses,
                TotalOnlineClasses = totalOnlineClasses,
                TotalUngradedAssignments = totalUngradedAssignments,
                TotalQuestionsReceived = totalQuestionsReceived
            };
        }
    }
}