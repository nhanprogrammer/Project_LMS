using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;
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
            // Bước 1: Lấy danh sách phân công giảng dạy của giáo viên
            var teachingAssignments = await _reportRepository.GetTeachingAssignmentsByTeacherAsync(teacherId);

            // Bước 2: Lấy danh sách các ID của năm học
            var academicYearIds = teachingAssignments
                .Where(ta => ta.Class?.AcademicYearId.HasValue == true)
                .Select(ta => ta.Class.AcademicYearId.Value)
                .Distinct()
                .ToList();

            // Bước 3: Lấy danh sách các học kỳ
            var semesters = await _reportRepository.GetSemestersByAcademicYearIdsAsync(academicYearIds);

            // Bước 4: Lấy buổi học đầu tiên cho các lớp và môn học
            var classTeachingDetailsWithLesson = new List<(TeachingAssignment Ta, Lesson? FirstLesson)>();
            foreach (var ta in teachingAssignments)
            {
                var firstLesson = await _reportRepository.GetFirstLessonByClassAndSubjectAsync(ta.ClassId ?? 0, ta.SubjectId ?? 0, teacherId);
                classTeachingDetailsWithLesson.Add((ta, firstLesson));
            }

            // Bước 5: Nhóm theo học kỳ và tạo response
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
                    SemesterName = (g.Key.SemesterName ?? "Unknown Semester") + " - " +
                        (g.Key.StartDate.HasValue && g.Key.EndDate.HasValue && g.Key.StartDate <= g.Key.EndDate
                            ? (g.Key.StartDate.Value.Year == g.Key.EndDate.Value.Year
                                ? g.Key.StartDate.Value.ToString("yyyy")
                                : g.Key.StartDate.Value.ToString("yyyy") + "-" + g.Key.EndDate.Value.ToString("yyyy"))
                            : "Unknown Year"),
                    AcademicYear = g.Key.AcademicYear != null
                        ? $"{g.Key.AcademicYear.StartDate:yyyy} - {g.Key.AcademicYear.EndDate:yyyy}"
                        : "Unknown Year",
                    ClassTeachingDetails = classTeachingDetailsWithLesson
                        .Where(ta => ta.Ta.Class?.AcademicYearId == g.Key.AcademicYear?.Id &&
                                    ta.Ta.StartDate.HasValue &&
                                    g.Key.StartDate.HasValue &&
                                    g.Key.EndDate.HasValue &&
                                    ta.Ta.StartDate.Value >= g.Key.StartDate.Value &&
                                    ta.Ta.StartDate.Value <= g.Key.EndDate.Value)
                        //Thời gian bắt đầu và kết thúc của phân công giảng dạy phải nằm trong thời gian học kỳ
                        // thì mới thêm vào response của ClassTeachingDetail được
                        .Select(ta => new ClassTeachingDetail
                        {
                            ClassId = ta.Ta.ClassId ?? 0,
                            ClassName = ta.Ta.Class?.Name ?? "Unknown Class",
                            SubjectName = ta.Ta.Subject?.SubjectName ?? "Unknown Subject",
                            StartDate = ta.Ta.StartDate,
                            EndDate = ta.Ta.EndDate,
                            ClassStatus = ta.Ta.EndDate.HasValue && DateTime.Now > ta.Ta.EndDate.Value
                            ? "Đã hoàn thành"
                            : (ta.Ta.StartDate.HasValue && DateTime.Now < ta.Ta.StartDate.Value
                                ? "Chưa bắt đầu"
                                : "Chưa hoàn thành"),
                            FirstSchedule = ta.FirstLesson != null
                                ? new ClassScheduleDetail
                                {
                                    StartTime = ta.FirstLesson.StartDate ?? DateTime.MinValue,
                                    EndTime = ta.FirstLesson.EndDate ?? DateTime.MinValue
                                }
                                : null // Gán buổi học đầu tiên (nếu có)
                        }).ToList()
                })
                .ToList();

            return groupedBySemester;
        }

        public async Task<StudentClassStatisticsResponse> GetStudentClassStatisticsAsync(int studentId)
        {
            var classStudents = await _reportRepository.GetClassStudentsByStudentIdAsync(studentId);

            int completedClasses = 0;
            int ongoingClasses = 0;
            double totalScore = 0;
            double totalCoefficient = 0;

            foreach (var classStudent in classStudents)
            {
                var academicYear = classStudent.Class?.AcademicYear;
                if (academicYear != null)
                {
                    if (academicYear.EndDate.HasValue && DateTime.Now > academicYear.EndDate.Value)
                    {
                        completedClasses++;
                    }
                    else
                    {
                        ongoingClasses++;
                    }
                }

                var assignments = await _reportRepository.GetAssignmentsByStudentAndClassAsync(studentId, classStudent.ClassId ?? 0);

                foreach (var assignment in assignments)
                {
                    if (assignment.TestExam?.TestExamType != null)
                    {
                        var coefficient = assignment.TestExam.TestExamType.Coefficient ?? 1;
                        totalScore += (assignment.TotalScore ?? 0) * coefficient;
                        totalCoefficient += coefficient;
                    }
                }
            }

            double averageScore = totalCoefficient > 0 ? totalScore / totalCoefficient : 0;

            return new StudentClassStatisticsResponse
            {
                TotalClasses = classStudents.Count,
                CompletedClasses = completedClasses,
                OngoingClasses = ongoingClasses,
                AverageScore = averageScore
            };
        }

        public async Task<StudentSubjectStatisticsResponse> GetStudentSubjectStatisticsAsync(int studentId)
        {
            var classStudents = await _reportRepository.GetClassStudentsByStudentIdAsync(studentId);

            int completedSubjects = 0;
            int ongoingSubjects = 0;
            var subjectIds = new HashSet<int>();

            foreach (var classStudent in classStudents)
            {
                var academicYear = classStudent.Class?.AcademicYear;
                if (academicYear != null)
                {
                    var isCompleted = academicYear.EndDate.HasValue && DateTime.Now > academicYear.EndDate.Value;

                    var subjects = await _reportRepository.GetSubjectsByClassIdAsync(classStudent.ClassId ?? 0);

                    foreach (var subject in subjects)
                    {
                        if (!subjectIds.Contains(subject.Id))
                        {
                            subjectIds.Add(subject.Id);

                            if (isCompleted)
                                completedSubjects++;
                            else
                                ongoingSubjects++;
                        }
                    }
                }
            }

            return new StudentSubjectStatisticsResponse
            {
                TotalSubjects = subjectIds.Count,
                CompletedSubjects = completedSubjects,
                OngoingSubjects = ongoingSubjects
            };
        }

        public async Task<List<StudentSemesterStatisticsResponse>> GetStudentSemesterStatisticsAsync(int studentId)
        {
            var classStudents = await _reportRepository.GetClassStudentsByStudentIdAsync(studentId);

            var academicYearIds = classStudents
                .Where(cs => cs.Class?.AcademicYearId.HasValue == true && cs.IsActive == true && cs.IsDelete == false)
                .Select(cs => cs.Class.AcademicYearId.Value)
                .ToList();

            var semesters = await _reportRepository.GetSemestersByAcademicYearIdsAsync(academicYearIds);

            var classSubjectDetails = new List<(ClassStudent Cs, ClassSubject Subject, Lesson? FirstLesson)>();

            foreach (var cs in classStudents)
            {
                var subjects = await _reportRepository.GetClassSubjectsWithSubjectsByClassIdAsync(cs.ClassId ?? 0);

                foreach (var subject in subjects)
                {
                    var firstLesson = await _reportRepository.GetFirstLessonByClassAndSubjectAsync(cs.ClassId ?? 0, subject.SubjectId ?? 0);
                    classSubjectDetails.Add((cs, subject, firstLesson));
                }
            }
            foreach (var classSubjectDetail in classSubjectDetails)
            {
                Console.WriteLine($"ClassId: {classSubjectDetail.Cs.ClassId}, SubjectId: {classSubjectDetail.Subject.SubjectId}, FirstLesson: {classSubjectDetail.FirstLesson?.StartDate}");
            }
            var groupedBySemester = semesters
            .GroupBy(s => new
            {
                SemesterId = s.Id,
                SemesterName = s.Name,
                AcademicYear = s.AcademicYear,
                StartDate = s.StartDate,
                EndDate = s.EndDate
            })
            .Select(g =>
            {
                var classDetails = classSubjectDetails
                    .Where(cs => cs.Cs.Class?.AcademicYearId == g.Key.AcademicYear?.Id)
                    .Select(cs => new StudentClassDetail
                    {
                        ClassId = cs.Cs.ClassId ?? 0,
                        ClassName = cs.Cs.Class?.Name ?? "Unknown Class",
                        SubjectName = cs.Subject.Subject?.SubjectName ?? "Unknown Subject",
                        Status = cs.Cs.Class != null && cs.Cs.Class.EndDate.HasValue && DateTime.Now > cs.Cs.Class.EndDate.Value
                            ? "Đã hoàn thành"
                            : (cs.Cs.Class?.StartDate.HasValue == true && DateTime.Now < cs.Cs.Class.StartDate.GetValueOrDefault()
                                ? "Chưa bắt đầu"
                                : "Chưa hoàn thành"),
                        FirstSchedule = cs.FirstLesson != null
                            ? new ClassScheduleDetail
                            {
                                StartTime = cs.FirstLesson.StartDate ?? DateTime.MinValue,
                                EndTime = cs.FirstLesson.EndDate ?? DateTime.MinValue,
                            }
                            : null
                    })
                    .ToList();

                // Nếu không có dữ liệu, thêm một phần tử chỉ ra rằng không có dữ liệu
                if (!classDetails.Any())
                {
                    classDetails.Add(new StudentClassDetail
                    {
                        ClassId = 0,
                        ClassName = "Không có lớp học",
                        SubjectName = "Không có môn học",
                        Status = "Không có dữ liệu",
                        FirstSchedule = null
                    });
                }

                return new StudentSemesterStatisticsResponse
                {
                    SemesterName = (g.Key.SemesterName ?? "Unknown Semester") + " - " +
                        (g.Key.StartDate.HasValue && g.Key.EndDate.HasValue && g.Key.StartDate <= g.Key.EndDate
                            ? (g.Key.StartDate.Value.Year == g.Key.EndDate.Value.Year
                                ? g.Key.StartDate.Value.ToString("yyyy")
                                : g.Key.StartDate.Value.ToString("yyyy") + "-" + g.Key.EndDate.Value.ToString("yyyy"))
                            : "Unknown Year"),
                    AcademicYear = g.Key.AcademicYear != null
                        ? $"{g.Key.AcademicYear.StartDate:yyyy} - {g.Key.AcademicYear.EndDate:yyyy}"
                        : "Unknown Year",
                    ClassDetails = classDetails
                };
            })
            .ToList();
            return groupedBySemester;
        }
    }
}