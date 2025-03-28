using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;
using Spire.Doc;

namespace Project_LMS.Repositories;

public class GradeEntryRepository : IGradeEntryRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GradeEntryRepository> _logger;

    public GradeEntryRepository(ApplicationDbContext context, ILogger<GradeEntryRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GradingDataResponse> GetGradingDataAsync(int testId, int teacherId)
    {
        try
        {
            // Lấy thông tin giáo viên
            var teacher = await _context.Users.FindAsync(teacherId);
            if (teacher == null)
            {
                throw new Exception("Không tìm thấy giáo viên!");
            }

            // Lấy thông tin bài kiểm tra hoặc lịch thi
            var test = await _context.TestExams
                .Include(t => t.Subject)
                .FirstOrDefaultAsync(t => t.Id == testId && t.IsDelete == false);
            if (test == null)
            {
                throw new Exception("Không tìm thấy bài kiểm tra hoặc lịch thi!");
            }

            // Kiểm tra ClassId
            if (!test.ClassId.HasValue)
            {
                throw new Exception("Bài kiểm tra không được liên kết với lớp học nào!");
            }

            int classId = test.ClassId.Value;

            // Kiểm tra quyền truy cập
            var classInfo = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == classId && c.IsDelete == false);
            if (classInfo == null)
            {
                throw new Exception("Không tìm thấy lớp học!");
            }

            // Kiểm tra xem lớp có học môn của bài kiểm tra/lịch thi không
            var classSubject = await _context.ClassSubjects
                .FirstOrDefaultAsync(cs =>
                    cs.ClassId == classId && cs.SubjectId == test.SubjectId && cs.IsDelete == false);
            if (classSubject == null)
            {
                throw new Exception("Lớp này không học môn của bài kiểm tra/lịch thi!");
            }

            // Kiểm tra quyền truy cập
            bool hasAccess = false;
            if (teacher.RoleId == 1) // Admin
            {
                hasAccess = true;
            }
            else if (classInfo.UserId == teacherId) // Giáo viên chủ nhiệm
            {
                hasAccess = true;
            }
            else
            {
                // Kiểm tra phân công giảng dạy tại thời điểm bài kiểm tra
                var teachingAssignment = await _context.TeachingAssignments
                    .FirstOrDefaultAsync(ta =>
                        ta.UserId == teacherId && ta.ClassId == classId && ta.SubjectId == test.SubjectId &&
                        ta.IsDelete == false);
                if (teachingAssignment != null &&
                    teachingAssignment.StartDate <= test.StartDate &&
                    teachingAssignment.EndDate >= test.StartDate)
                {
                    hasAccess = true;
                }
            }

            if (!hasAccess)
            {
                throw new Exception("Bạn không có quyền chấm điểm cho lớp này!");
            }

            // Kiểm tra trạng thái bài kiểm tra/lịch thi
            if (test.ScheduleStatusId == null)
            {
                _logger.LogWarning($"TestId: {testId} không có ScheduleStatusId, giả định là 5 (Đã hoàn thành).");
                test.ScheduleStatusId = 5; // Giả định để vượt qua kiểm tra
            }

            if (test.ScheduleStatusId == 1) // 1 = Chờ phê duyệt
            {
                throw new Exception("Bài kiểm tra/lịch thi chưa được phê duyệt, không thể chấm điểm!");
            }

            // Chỉ cho phép chấm điểm khi trạng thái là "Đã hoàn thành" (5) hoặc "Đã kết thúc" (7)
            if (test.ScheduleStatusId != 5 && test.ScheduleStatusId != 7)
            {
                throw new Exception("Bài kiểm tra/lịch thi chưa hoàn thành hoặc chưa kết thúc, không thể chấm điểm!");
            }

            // Nếu là lịch thi (is_exam = true), áp dụng quy tắc bổ sung
            if (test.IsExam == null)
            {
                _logger.LogWarning($"TestId: {testId} không có IsExam, giả định là false.");
                test.IsExam = false; // Giả định để vượt qua kiểm tra
            }

            if (test.IsExam == true)
            {
                // Yêu cầu quyền đặc biệt cho lịch thi
                if (teacher.RoleId != 1 && classInfo.UserId != teacherId)
                {
                    throw new Exception("Chỉ Admin hoặc giáo viên chủ nhiệm mới có thể chấm điểm kỳ thi!");
                }
            }

            // Lấy danh sách học sinh trong lớp tại thời điểm bài kiểm tra
            var classStudents = await _context.ClassStudents
                .Where(cs => cs.ClassId == classId &&
                             cs.IsActive == true &&
                             cs.IsDelete == false &&
                             cs.CreateAt <= test.StartDate)
                .ToListAsync();

            // Lấy thông tin học sinh từ bảng users
            var studentIds = classStudents
                .Where(cs => cs.UserId.HasValue)
                .Select(cs => cs.UserId.Value)
                .ToList();
            var students = await _context.Users
                .Where(u => studentIds.Contains(u.Id) && u.IsDelete == false)
                .ToListAsync();

            // Kiểm tra số lượng học sinh
            if (students.Count != classInfo.StudentCount)
            {
                _logger.LogWarning(
                    $"Số lượng học sinh trong lớp {classId} không khớp: Expected {classInfo.StudentCount}, Found {students.Count}");
            }

            // Lấy bài nộp của học sinh
            var assignments = await _context.Assignments
                .Where(a => a.TestExamId == testId && a.IsDelete == false)
                .ToListAsync();

            // Ghi log để kiểm tra bài nộp
            if (!assignments.Any())
            {
                _logger.LogInformation($"Không tìm thấy bài nộp nào cho TestId: {testId}");
            }
            else
            {
                foreach (var assignment in assignments)
                {
                    _logger.LogInformation(
                        $"Bài nộp của học sinh {assignment.UserId} cho TestId: {testId}, SubmissionFile: {assignment.SubmissionFile}");
                }
            }

            // Tạo danh sách học sinh và điểm số
            var studentGrades = classStudents
                .Where(cs => cs.UserId.HasValue)
                .Join(students,
                    cs => cs.UserId.Value,
                    s => s.Id,
                    (cs, s) => new { ClassStudent = cs, Student = s })
                .Select(joined => new StudentGradeResponse
                {
                    StudentId = joined.Student.Id,
                    StudentName = joined.Student.FullName,
                    Score = assignments.FirstOrDefault(a => a.UserId == joined.Student.Id)?.TotalScore,
                    SubmissionStatus = assignments.Any(a => a.UserId == joined.Student.Id && a.IsSubmit == true)
                        ? "Đã nộp"
                        : "Chưa nộp",
                    SubmissionFile = assignments.FirstOrDefault(a => a.UserId == joined.Student.Id)?.SubmissionFile,
                    Comment = assignments.FirstOrDefault(a => a.UserId == joined.Student.Id)?.Comment,
                    ClassStatus = joined.ClassStudent.IsActive == true && joined.ClassStudent.IsDelete == false
                        ? "Đang học"
                        : "Đã rời lớp"
                }).ToList();

            // Chuyển đổi Duration từ định dạng "HH:mm:ss" sang TimeOnly
            TimeOnly duration;
            try
            {
                if (test.Duration.HasValue)
                {
                    duration = test.Duration.Value;
                }
                else
                {
                    _logger.LogWarning($"TestId: {testId} không có Duration, sử dụng giá trị mặc định 0 phút.");
                    duration = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(0));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    $"Không thể chuyển đổi Duration '{test.Duration}' sang TimeOnly, sử dụng giá trị mặc định 0 phút.");
                duration = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(0));
            }

            // Đọc nội dung đề tài từ file attachment (nếu có)
            string proposalContent = await ExtractProposalContentFromFileAsync(test.Attachment);

            // Tạo dữ liệu trả về
            var gradingData = new GradingDataResponse
            {
                TestId = test.Id,
                TestName = test.Topic ?? "Unknown", // Xử lý trường hợp Topic là null
                ClassId = test.ClassId,
                ClassName = classInfo.Name,
                Subject = test.Subject?.SubjectName ?? "Unknown", // Xử lý trường hợp Subject là null
                StartTime = test.StartDate ?? DateTimeOffset.MinValue,
                EndTime = test.EndDate ?? DateTimeOffset.MinValue,
                Duration = duration,
                Form = test.Form ?? "Unknown", // Xử lý trường hợp Form là null
                Description = test.Description,
                Attachment = test.Attachment,
                IsExam = test.IsExam ?? false, // Xử lý trường hợp IsExam là null
                ProposalContent = proposalContent,
                StudentGrades = studentGrades
            };

            return gradingData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy dữ liệu chấm điểm: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<bool> SaveGradesAsync(SaveGradesRequest request, int teacherId)
    {
        try
        {
            // Kiểm tra thông tin giáo viên
            var teacher = await _context.Users.FindAsync(teacherId);
            if (teacher == null)
            {
                throw new Exception("Không tìm thấy giáo viên!");
            }

            // Kiểm tra bài kiểm tra hoặc lịch thi
            var test = await _context.TestExams
                .Include(t => t.Subject)
                .FirstOrDefaultAsync(t => t.Id == request.TestId && t.IsDelete == false);
            if (test == null)
            {
                throw new Exception("Không tìm thấy bài kiểm tra hoặc lịch thi!");
            }

            // Kiểm tra ClassId của bài kiểm tra
            if (!test.ClassId.HasValue)
            {
                throw new Exception("Bài kiểm tra không được liên kết với lớp học nào!");
            }

            int classId = test.ClassId.Value;

            // Kiểm tra thông tin lớp học
            var classInfo = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == classId && c.IsDelete == false);
            if (classInfo == null)
            {
                throw new Exception("Không tìm thấy lớp học!");
            }

            // Kiểm tra lớp có học môn của bài kiểm tra không
            var classSubject = await _context.ClassSubjects
                .FirstOrDefaultAsync(cs =>
                    cs.ClassId == classId && cs.SubjectId == test.SubjectId && cs.IsDelete == false);
            if (classSubject == null)
            {
                throw new Exception("Lớp này không học môn của bài kiểm tra/lịch thi!");
            }

            // Kiểm tra quyền truy cập
            bool hasAccess = await HasGradingPermissionAsync(teacherId, classId, test.SubjectId.Value,
                test.StartDate.Value,
                test.IsExam.Value, classInfo.UserId);
            if (!hasAccess)
            {
                throw new Exception("Bạn không có quyền chấm điểm cho lớp này!");
            }

            // Kiểm tra trạng thái bài kiểm tra
            var allowedStatuses = new[] { 5, 7 }; // Có thể thay bằng enum hoặc cấu hình
            if (!allowedStatuses.Contains<int>(test.ScheduleStatusId.Value))
            {
                throw new Exception("Bài kiểm tra/lịch thi chưa hoàn thành hoặc chưa kết thúc, không thể chấm điểm!");
            }

            // Kiểm tra thời gian kết thúc bài kiểm tra
            if (test.EndDate > DateTimeOffset.UtcNow)
            {
                throw new Exception("Bài kiểm tra/lịch thi chưa kết thúc, không thể chấm điểm!");
            }

            // Lấy danh sách học sinh hợp lệ trong lớp
            var classStudents = await _context.ClassStudents
                .Where(cs =>
                    cs.ClassId == classId && cs.IsActive == true && cs.IsDelete == false &&
                    cs.CreateAt <= test.StartDate)
                .Select(cs => cs.UserId.Value)
                .ToListAsync();

            // Lấy tất cả bài nộp cho bài kiểm tra này
            var assignments = await _context.Assignments
                .Where(a => a.TestExamId == request.TestId && a.IsDelete == false)
                .ToListAsync();

            // Lưu điểm cho từng học sinh
            foreach (var grade in request.Grades)
            {
                // Kiểm tra học sinh có thuộc lớp không
                if (!classStudents.Contains(grade.StudentId))
                {
                    throw new Exception($"Học sinh {grade.StudentId} không thuộc lớp này tại thời điểm bài kiểm tra!");
                }

                // Kiểm tra điểm hợp lệ
                if (grade.Score.HasValue && (grade.Score < 0 || grade.Score > 10))
                {
                    throw new Exception($"Điểm của học sinh {grade.StudentId} không hợp lệ (phải từ 0 đến 10)!");
                }

                // Tìm bài nộp của học sinh
                var assignment = assignments.FirstOrDefault(a => a.UserId == grade.StudentId);
                if (assignment == null)
                {
                    // Thêm mới bài nộp nếu chưa tồn tại
                    assignment = new Assignment
                    {
                        TestExamId = request.TestId,
                        UserId = grade.StudentId,
                        TotalScore = grade.Score ?? 0,
                        Comment = grade.Comment,
                        IsSubmit = false,
                        StatusAssignmentId = 1,
                        CreateAt = DateTime.UtcNow,
                        UpdateAt = DateTime.UtcNow,
                        UserCreate = teacherId,
                        UserUpdate = teacherId,
                        IsDelete = false
                    };
                    _context.Assignments.Add(assignment);
                }
                else
                {
                    // Cập nhật bài nộp nếu đã tồn tại
                    assignment.TotalScore = grade.Score ?? 0;
                    assignment.Comment = grade.Comment;
                    assignment.UpdateAt = DateTime.UtcNow;
                    assignment.UserUpdate = teacherId;
                }
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lưu điểm cho TestId: {TestId}, TeacherId: {TeacherId}", request.TestId,
                teacherId);
            throw;
        }
    }

// Hàm phụ kiểm tra quyền truy cập (giữ nguyên)
    private async Task<bool> HasGradingPermissionAsync(int teacherId, int classId, int subjectId,
        DateTimeOffset testStartDate, bool isExam, int? classTeacherId)
    {
        var teacher = await _context.Users.FindAsync(teacherId);
        if (teacher == null) return false;

        if (teacher.RoleId == 1) return true; // Admin có toàn quyền

        if (classTeacherId == teacherId) return true; // Giáo viên chủ nhiệm

        if (isExam) return false; // Kỳ thi chỉ cho Admin hoặc giáo viên chủ nhiệm

        // Kiểm tra phân công giảng dạy
        var teachingAssignment = await _context.TeachingAssignments
            .FirstOrDefaultAsync(ta =>
                ta.UserId == teacherId && ta.ClassId == classId && ta.SubjectId == subjectId && ta.IsDelete == false);
        if (teachingAssignment != null && teachingAssignment.StartDate <= testStartDate &&
            teachingAssignment.EndDate >= testStartDate)
        {
            return true;
        }

        return false;
    }

    private async Task<string> ExtractProposalContentFromFileAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl))
        {
            return null;
        }

        try
        {
            // Kiểm tra định dạng file dựa trên phần mở rộng
            var fileExtension = Path.GetExtension(fileUrl).ToLower();
            if (fileExtension != ".doc" && fileExtension != ".docx")
            {
                _logger.LogWarning($"Chỉ hỗ trợ định dạng .doc và .docx: {fileUrl}");
                return "Chỉ hỗ trợ định dạng .doc và .docx.";
            }

            using (var httpClient = new HttpClient())
            {
                // Lấy byte array từ URL của file
                byte[] fileBytes = await httpClient.GetByteArrayAsync(fileUrl);

                // Sử dụng Spire.Doc để đọc file
                using (MemoryStream stream = new MemoryStream(fileBytes))
                {
                    Document document = new Document(stream);
                    string content = document.GetText();
                    return ExtractProposalTitle(content);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi đọc nội dung file từ URL {fileUrl}: {ex.Message}");
            return "Không thể đọc nội dung file.";
        }
    }

    private string ExtractProposalTitle(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return "Không có nội dung để trích xuất.";
        }

        // Tách nội dung thành các dòng
        var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            // Chuẩn hóa chuỗi để xử lý khoảng trắng và không phân biệt hoa/thường
            var normalizedLine = System.Text.RegularExpressions.Regex.Replace(trimmedLine, @"\s+", " ");

            // Kiểm tra dòng "Đề bài" (không phân biệt hoa/thường)
            var proposalPattern = new Regex(@"^Đề\s*bài\s*:?\s*(.*)", RegexOptions.IgnoreCase);
            var match = proposalPattern.Match(normalizedLine);
            if (match.Success)
            {
                // Lấy phần nội dung sau "Đề bài :" (nhóm 1 trong regex)
                var proposalTitle = match.Groups[1].Value.Trim();
                if (string.IsNullOrEmpty(proposalTitle))
                {
                    return "Không tìm thấy tên đề bài.";
                }

                return proposalTitle;
            }
        }

        // Nếu không tìm thấy dòng "Đề bài"
        return "Không tìm thấy dòng Đề bài.";
    }
}