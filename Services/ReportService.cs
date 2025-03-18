using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
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
        private readonly ApplicationDbContext _context;

        public ReportService(IReportRepository reportRepository, ApplicationDbContext context)
        {
            _reportRepository = reportRepository;
            _context = context;
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

        public async Task<TeacherPerformanceReport> GetTeacherPerformanceReportAsync(int userId)
        {
            var totalClasses = await _reportRepository.GetTotalClassesByTeacherAsync(userId);
            var classPerformanceReports = await _reportRepository.GetClassPerformanceByTeacherAsync(userId);

            var totalExcellentStudents = classPerformanceReports.Sum(r => r.ExcellentCount);
            var totalGoodStudents = classPerformanceReports.Sum(r => r.GoodCount);
            var totalAverageStudents = classPerformanceReports.Sum(r => r.AverageCount);
            var totalWeakStudents = classPerformanceReports.Sum(r => r.WeakCount);

            return new TeacherPerformanceReport
            {
                TotalClasses = totalClasses,
                TotalExcellentStudents = totalExcellentStudents,
                TotalGoodStudents = totalGoodStudents,
                TotalAverageStudents = totalAverageStudents,
                TotalWeakStudents = totalWeakStudents
            };
        }

        public async Task<List<TeacherSemesterStatisticsResponse>> GetTeacherSemesterStatisticsAsync(int teacherId)
        {
            var teachingAssignments = await _reportRepository.GetTeachingAssignmentsByTeacherAsync(teacherId);

            var academicYearIds = teachingAssignments
                .Where(ta => ta.Class?.AcademicYearId.HasValue == true)
                .Select(ta => ta.Class.AcademicYearId.Value)
                .Distinct()
                .ToList();

            var semesters = await _context.AcademicYears
                .Where(ay => academicYearIds.Contains(ay.Id))
                .Include(ay => ay.Semesters)
                .SelectMany(ay => ay.Semesters)
                .ToListAsync();

            var groupedBySemester = semesters
                .GroupBy(s => new
                {
                    SemesterId = s.Id,
                    SemesterName = s.Name,
                    AcademicYear = s.AcademicYear,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate
                })
                .Select(g => new TeacherSemesterStatisticsResponse
                {
                    SemesterName = g.Key.SemesterName ?? "Unknown Semester",
                    AcademicYear = g.Key.AcademicYear != null
                        ? $"{g.Key.AcademicYear.StartDate:yyyy} - {g.Key.AcademicYear.EndDate:yyyy}"
                        : "Unknown Year",
                    ClassTeachingDetails = teachingAssignments
                        .Where(ta => ta.Class?.AcademicYearId == g.Key.AcademicYear?.Id &&
                                    ta.StartDate.HasValue &&
                                    g.Key.StartDate.HasValue &&
                                    g.Key.EndDate.HasValue &&
                                    ta.StartDate.Value >= g.Key.StartDate.Value &&
                                    ta.StartDate.Value <= g.Key.EndDate.Value)
                        .Select(ta => new ClassTeachingDetail
                        {
                            ClassId = ta.ClassId ?? 0,
                            ClassName = ta.Class?.Name ?? "Unknown Class",
                            SubjectName = ta.Subject?.SubjectName ?? "Unknown Subject",
                            StartDate = ta.StartDate,
                            EndDate = ta.EndDate,
                            ClassStatus = ta.Class?.IsDelete == false ? "Active" : "Inactive"
                        }).ToList()
                })
                .ToList();

            return groupedBySemester;
        }
    }
}