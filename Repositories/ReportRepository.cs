using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AcademicYear?> GetAcademicYearByIdAsync(int academicYearId)
        {
            return await _context.AcademicYears.FirstOrDefaultAsync(ay => ay.Id == academicYearId);
        }

        public async Task<int> GetTotalStudentsAsync(int academicYearId)
        {
            return await _context.ClassStudents
                .Include(cs => cs.Class)
                .CountAsync(cs => cs.Class != null && cs.Class.AcademicYearId == academicYearId && cs.IsDelete == false);
        }

        public async Task<int> GetTotalTeachersAsync(int academicYearId)
        {
            return await _context.Classes
                .Where(c => c.AcademicYearId == academicYearId && c.IsDelete == false)
                .Select(c => c.UserId)
                .Distinct()
                .CountAsync();
        }

        public async Task<int> GetTotalClassesAsync(int academicYearId)
        {
            return await _context.Classes.CountAsync(c => c.AcademicYearId == academicYearId && (c.IsDelete == false));
        }

        public async Task<List<ClassPerformanceReport>> GetClassPerformanceReportAsync(int academicYearId, int departmentId)
        {
            var classes = await _context.Classes
                .Where(c => c.AcademicYearId == academicYearId && c.DepartmentId == departmentId && c.IsDelete == false)
                .ToListAsync();

            var report = new List<ClassPerformanceReport>();

            foreach (var classEntity in classes)
            {
                var students = await _context.ClassStudents
                    .Where(cs => cs.ClassId == classEntity.Id && cs.IsDelete == false)
                    .Include(cs => cs.User)
                    .ToListAsync();

                int excellentCount = 0;
                int goodCount = 0;
                int averageCount = 0;
                int weakCount = 0;

                foreach (var student in students)
                {
                    var assignments = await _context.Assignments
                        .Where(a => a.UserId == student.UserId && a.IsDelete == false)
                        .Include(a => a.TestExam)
                        .ThenInclude(te => te.TestExamType)
                        .ToListAsync();

                    double totalScore = 0;
                    double totalCoefficient = 0;

                    foreach (var assignment in assignments)
                    {
                        if (assignment.TestExam?.TestExamType != null)
                        {
                            var coefficient = assignment.TestExam.TestExamType.Coefficient ?? 1;
                            totalScore += (assignment.TotalScore ?? 0) * coefficient;
                            totalCoefficient += coefficient;
                        }
                    }

                    double averageScore = totalCoefficient > 0 ? totalScore / totalCoefficient : 0;

                    if (averageScore >= 8.5)
                    {
                        excellentCount++;
                    }
                    else if (averageScore >= 7.0)
                    {
                        goodCount++;
                    }
                    else if (averageScore >= 5.0)
                    {
                        averageCount++;
                    }
                    else
                    {
                        weakCount++;
                    }
                }

                report.Add(new ClassPerformanceReport
                {
                    ClassId = classEntity.Id,
                    ClassName = classEntity.Name ?? "Unknown",
                    ExcellentCount = excellentCount,
                    GoodCount = goodCount,
                    AverageCount = averageCount,
                    WeakCount = weakCount
                });
            }

            return report;
        }
        public async Task<SchoolLevelStatisticsResponse> GetSchoolLevelStatisticsAsync(int academicYearId, bool isJuniorHigh)
        {

            var allowedDepartmentIds = isJuniorHigh ? new[] { "K06", "K07", "K08", "K09" } : new[] { "K10", "K11", "K12" };

            var gradeStatistics = await (from cs in _context.ClassStudents
                                         join c in _context.Classes
                                         on cs.ClassId equals c.Id
                                         where cs.IsDelete == false
                                               && c.AcademicYearId == academicYearId
                                               && c.IsDelete == false
                                               && c.Department != null
                                               && allowedDepartmentIds.Contains(c.Department.DepartmentCode ?? "")
                                            group c by c.Department.DepartmentCode into g
                                         select new GradeStatistics
                                         {
                                             DepartmentCode = g.Key.ToString(),
                                             TotalStudents = g.Count()
                                         })
                                       .ToListAsync();

            var allGradeStatistics = allowedDepartmentIds
                .Select(deptCode =>
                {
                    return new GradeStatistics
                    {
                        DepartmentCode = deptCode,
                        TotalStudents = gradeStatistics.FirstOrDefault(gs => gs.DepartmentCode == deptCode)?.TotalStudents ?? 0
                    };
                })
                .OrderBy(gs => gs.DepartmentCode)
                .ToList();

            return new SchoolLevelStatisticsResponse
            {
                AcademicYearId = academicYearId,
                SchoolLevel = isJuniorHigh ? "Junior High School" : "High School",
                GradeStatistics = allGradeStatistics
            };
        }

    }
}