using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class TranscriptService : ITranscriptService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public TranscriptService(ApplicationDbContext context, ILogger<TranscriptService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<List<TranscriptReponse>>> GetTranscriptAsync(TranscriptRequest transcriptRequest, int userId)
        {
            try
            {
                // Lọc danh sách sinh viên theo điều kiện từ request
                var transcriptDetails = await _context.Users
                    .Include(u => u.Assignments)
                        .ThenInclude(a => a.TestExam)
                            .ThenInclude(te => te.TestExamType)
                    .Include(u => u.Assignments)
                        .ThenInclude(a => a.TestExam)
                            .ThenInclude(te => te.Semesters)
                    .Where(u => u.Assignments.Any(a =>
                        (transcriptRequest.AcademicYearId == 0 || a.TestExam.Semesters.AcademicYearId == transcriptRequest.AcademicYearId) &&
                        (transcriptRequest.ClassId == 0 || a.TestExam.ClassId == transcriptRequest.ClassId) &&
                        (transcriptRequest.SubjectId == 0 || a.TestExam.SubjectId == transcriptRequest.SubjectId) &&
                        (transcriptRequest.DepartmentId == 0 || a.TestExam.DepartmentId == transcriptRequest.DepartmentId)
                    ))
                    .Select(u => new TranscriptDetailReponse
                    {
                        semesterName = u.Assignments.FirstOrDefault() != null && u.Assignments.FirstOrDefault().TestExam != null && u.Assignments.FirstOrDefault().TestExam.Semesters != null ? u.Assignments.FirstOrDefault().TestExam.Semesters.Name : "N/A",
                        StudentName = u.ClassStudents != null ? u.ClassStudents.FirstOrDefault().User.FullName : "N/A",
                        DateOfBirth = u.ClassStudents != null && u.ClassStudents.FirstOrDefault().User.BirthDate.HasValue ? u.ClassStudents.FirstOrDefault().User.BirthDate.Value.ToString() : "N/A",
                        status = (u.Assignments.FirstOrDefault().TotalScore ?? 0) >= 5 ? "Passed" : "Failed",
                        updateAt = u.Assignments.FirstOrDefault().UpdateAt.HasValue ? u.Assignments.FirstOrDefault().UpdateAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : "N/A",
                        scoreReponses = u.Assignments.Select(a => new TranscriptDetailScoreReponse
                        {
                            pointTypeName = a.TestExam != null && a.TestExam.TestExamType != null ? a.TestExam.TestExamType.PointTypeName : "N/A",
                            coefficient = (int)(a.TestExam != null && a.TestExam.TestExamType != null ? a.TestExam.TestExamType.Coefficient : 0),
                            score = a.TotalScore ?? 0,
                            totalScore = (double)((a.TotalScore ?? 0) * (a.TestExam != null && a.TestExam.TestExamType != null ? a.TestExam.TestExamType.Coefficient : 1)),
                        }).ToList()
                    })
                    .ToListAsync();

                // Tính toán trung bình điểm theo hệ số
                foreach (var detail in transcriptDetails)
                {
                    if (detail.scoreReponses.Any())
                    {
                        double totalScore = detail.scoreReponses.Sum(s => s.totalScore);
                        double totalCoefficient = detail.scoreReponses.Sum(s => s.coefficient);
                        detail.totalYearScore = totalCoefficient > 0 ? totalScore / totalCoefficient : 0;
                    }
                }

                // Lấy thông tin lớp học có áp dụng bộ lọc
                var classStudentInfo = await _context.Classes
                    .Include(c => c.User)
                    .Include(c => c.ClassSubjects)
                        .ThenInclude(cs => cs.Subject)
                    .Where(c =>
                        (transcriptRequest.AcademicYearId == 0 || c.AcademicYearId == transcriptRequest.AcademicYearId) &&
                        (transcriptRequest.ClassId == 0 || c.Id == transcriptRequest.ClassId) &&
                        (transcriptRequest.SubjectId == 0 || c.ClassSubjects.Any(cs => cs.SubjectId == transcriptRequest.SubjectId)) &&
                        (transcriptRequest.DepartmentId == 0 || c.DepartmentId == transcriptRequest.DepartmentId)
                    )
                    .Select(c => new TranscriptReponse
                    {
                        StartTime = c.AcademicYear != null && c.AcademicYear.StartDate != null ? c.AcademicYear.StartDate.ToString() : "N/A",
                        ClassName = c.Name ?? "N/A",
                        ClassCode = c.ClassCode ?? "N/A",
                        SubjectName = c.ClassSubjects.FirstOrDefault() != null && c.ClassSubjects.FirstOrDefault().Subject != null ? c.ClassSubjects.FirstOrDefault().Subject.SubjectName : "N/A",
                        transcriptDetails = transcriptDetails
                    })
                    .ToListAsync();
                return new ApiResponse<List<TranscriptReponse>>(0, "Lấy thông tin bảng điểm thành công", classStudentInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTranscriptAsync");
                return new ApiResponse<List<TranscriptReponse>>(1, "Thất bại");
            }
        }



    }
}

