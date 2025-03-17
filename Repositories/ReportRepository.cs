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

        /// <summary>
        /// Đếm tổng số học sinh trong một niên khóa.
        /// </summary>
        public async Task<int> GetTotalStudentsAsync(int academicYearId)
        {
            return await _context.ClassStudents
                .Include(cs => cs.Class)
                .CountAsync(cs => cs.Class != null && cs.Class.AcademicYearId == academicYearId && cs.IsDelete == false);
        }

        /// <summary>
        /// Đếm tổng số giảng viên được phân công giảng dạy trong một niên khóa.
        /// </summary>
        public async Task<int> GetTotalTeachersAsync(int academicYearId)
        {
            return await _context.TeachingAssignments
                .Where(ta => ta.Class != null && ta.Class.AcademicYearId == academicYearId && ta.IsDelete == false)
                .Select(ta => ta.UserId)
                .Distinct()
                .CountAsync();
        }

        /// <summary>
        /// Đếm tổng số lớp học trong một niên khóa.
        /// </summary>
        public async Task<int> GetTotalClassesAsync(int academicYearId)
        {
            return await _context.Classes.CountAsync(c => c.AcademicYearId == academicYearId && (c.IsDelete == false));
        }

        /// <summary>
        /// Lấy báo cáo hiệu suất của các lớp trong một niên khóa và một phòng ban cụ thể.
        /// </summary>
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

        /// <summary>
        /// Lấy thống kê cấp trường dựa trên niên khóa và cấp học (trung học cơ sở hoặc trung học phổ thông).
        /// </summary>
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


        /// <summary>
        ///Thống kê teacher
        /// Đếm tổng số lớp mà một giảng viên được phân công giảng dạy.
        /// </summary>
        /// 
        public async Task<int> GetTotalClassesByTeacherAsync(int userId)
        {
            return await _context.TeachingAssignments
                .Where(ta => ta.UserId == userId && ta.IsDelete == false)
                .Select(ta => ta.ClassId)
                .Distinct()
                .CountAsync();
        }

        /// <summary>
        /// Đếm tổng số lớp học trực tuyến mà một giảng viên được phân công giảng dạy.
        /// </summary>
        public async Task<int> GetTotalOnlineClassesByTeacherAsync(int userId)
        {
            return await _context.ClassOnlines
                .Where(co => co.UserId == userId && co.IsDelete == false && co.ClassStatus == true)
                .Select(co => co.Id)
                .Distinct()
                .CountAsync();
        }


        /// <summary>
        /// Đếm tổng số bài kiểm tra chưa chấm mà một giảng viên chịu trách nhiệm.
        /// </summary>
        public async Task<int> GetTotalUngradedAssignmentsByTeacherAsync(int userId)
        {
            // Lấy danh sách TestExamId mà giảng viên được phân công chấm
            var testExamIds = await _context.Examiners
                .Where(e => e.UserId == userId && e.IsDelete == false)
                .Select(e => e.TestExamId)
                .ToListAsync();

            // Đếm số bài kiểm tra chưa chấm trong các TestExam của giảng viên
            var ungradedAssignmentsCount = await _context.Assignments
                .Where(a => testExamIds.Contains(a.TestExamId.Value)
                            && a.TotalScore == null
                            && a.IsDelete == false)
                .CountAsync();

            return ungradedAssignmentsCount;
        }

        /// <summary>
        /// Đếm tổng số câu hỏi mà một giảng viên nhận được từ sinh viên.
        /// </summary>
        public async Task<int> GetTotalQuestionsReceivedByTeacherAsync(int teacherId)
        {
            // Lấy danh sách teaching_assignment_id mà giảng viên phụ trách
            var teachingAssignments = await _context.TeachingAssignments
                .Where(ta => ta.UserId == teacherId && ta.IsDelete == false)
                .Select(ta => ta.Id)
                .ToListAsync();

            // Đếm số câu hỏi từ sinh viên gửi cho giảng viên (tức là không phải teacher đặt)
            var totalQuestions = await _context.QuestionAnswers
                .Where(qa => teachingAssignments.Contains(qa.TeachingAssignmentId.Value)
                            && qa.UserId != teacherId // Lọc bỏ câu hỏi do chính teacher tạo
                            && qa.QuestionsAnswerId == null // Chỉ lấy câu hỏi gốc, không phải câu trả lời
                            && qa.IsDelete == false)
                .CountAsync();

            return totalQuestions;
        }




    }
}