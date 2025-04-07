using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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

    public async Task<GradingDataResponse> GetGradingDataAsync(int testId, int teacherId, int? classId = null)
    {
        TestExam test = null;
        try
        {
            _logger.LogInformation(
                $"Getting grading data for TestId={testId}, TeacherId={teacherId}, ClassId={classId}");

            // Lấy thông tin giáo viên
            var teacher = await _context.Users.FindAsync(teacherId);
            if (teacher == null)
            {
                throw new Exception("Không tìm thấy giáo viên!");
            }

            // Lấy thông tin bài kiểm tra hoặc lịch thi
            test = await _context.TestExams
                .Include(t => t.Subject)
                .FirstOrDefaultAsync(t => t.Id == testId && t.IsDelete == false);
            if (test == null)
            {
                throw new Exception("Không tìm thấy bài kiểm tra hoặc lịch thi!");
            }

            // Lấy danh sách lớp liên kết với bài kiểm tra từ ClassTestExams
            var classTestExam = await _context.ClassTestExams
                .Where(cte => cte.TestExamId == testId && cte.IsDelete == false)
                .ToListAsync();

            _logger.LogInformation($"Số lượng lớp trong ClassTestExams cho TestId {testId}: {classTestExam.Count}");

            if (!classTestExam.Any())
            {
                _logger.LogWarning($"Không tìm thấy bản ghi nào trong ClassTestExams cho TestId {testId}");
                throw new Exception("Bài kiểm tra/lịch thi này chưa được gán cho lớp học nào trong ClassTestExams!");
            }


            // Xử lý classId: Chỉ sử dụng ClassTestExams
            int selectedClassId;
            if (classId.HasValue)
            {
                if (!classTestExam.Any(cte => cte.ClassId == classId.Value))
                {
                    _logger.LogWarning(
                        $"ClassId {classId.Value} không tồn tại trong ClassTestExams của TestId {testId}");
                    throw new Exception(
                        $"Lớp được chỉ định (ID: {classId.Value}) không liên kết với bài kiểm tra này!");
                }

                selectedClassId = classId.Value;
                _logger.LogInformation($"ClassId được chỉ định cho TestId {testId}: {selectedClassId}");
            }
            else
            {
                selectedClassId = classTestExam.First().ClassId.Value;
                _logger.LogInformation(
                    $"Không truyền ClassId, sử dụng ClassId đầu tiên từ ClassTestExams cho TestId {testId}: {selectedClassId}");
            }

            // Lấy thông tin lớp học
            var classInfo = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == selectedClassId && c.IsDelete == false);
            if (classInfo == null)
            {
                throw new Exception("Không tìm thấy lớp học!");
            }

            // Kiểm tra lớp có học môn của bài kiểm tra không
            var classSubject = await _context.ClassSubjects
                .FirstOrDefaultAsync(cs =>
                    cs.ClassId == selectedClassId && cs.SubjectId == test.SubjectId && cs.IsDelete == false);

            _logger.LogInformation(
                $"Kiểm tra ClassSubject: ClassId={selectedClassId}, SubjectId={test.SubjectId}, Found={classSubject != null}");

            if (classSubject == null)
            {
                _logger.LogError("Lớp {ClassId} không được gán môn học {SubjectId} của bài kiểm tra/lịch thi {TestId}!",
                    selectedClassId, test.SubjectId, test.Id);
                throw new Exception(
                    $"Lớp {selectedClassId} không được gán môn học (ID: {test.SubjectId}) của bài kiểm tra/lịch thi! Vui lòng thêm môn học này cho lớp hoặc chọn lớp khác.");
            }

            // Kiểm tra quyền truy cập
            bool hasAccess = await HasGradingPermissionAsync(teacherId, testId, test.IsExam ?? false);
            if (!hasAccess)
            {
                throw new Exception("Bạn không có quyền chấm điểm cho lớp này!");
            }

            // Kiểm tra trạng thái bài kiểm tra/lịch thi
            var endDateUtc = test.EndDate.HasValue ? test.EndDate.Value.ToUniversalTime() : DateTimeOffset.MaxValue;
            _logger.LogInformation($"TestId: {testId}, EndDate (UTC): {endDateUtc}, UtcNow: {DateTimeOffset.UtcNow}");

            if (endDateUtc > DateTimeOffset.UtcNow)
            {
                _logger.LogWarning(
                    $"Bài kiểm tra TestId: {testId} chưa kết thúc (EndDate: {endDateUtc}, Now: {DateTimeOffset.UtcNow})");
                throw new Exception("Bài kiểm tra hoặc kỳ thi này chưa kết thúc, không thể chấm điểm");
            }

            if (test.IsExam == null)
            {
                _logger.LogWarning($"TestId: {testId} không có IsExam, giả định là false.");
                test.IsExam = false;
            }

            // Lấy danh sách học sinh trong lớp tại thời điểm bài kiểm tra
            var classStudents = await _context.ClassStudents
                .Where(cs => cs.ClassId == selectedClassId &&
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

            if (students.Count != classInfo.StudentCount)
            {
                _logger.LogWarning(
                    $"Số lượng học sinh trong lớp {selectedClassId} không khớp: Expected {classInfo.StudentCount}, Found {students.Count}");
            }

            // Lấy bài nộp của học sinh
            var assignments = await _context.Assignments
                .Where(a => a.TestExamId == testId && a.IsDelete == false)
                .ToListAsync();
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

            var submittedAssignments = await _context.Assignments
                .Where(a => a.TestExamId == testId && a.IsDelete == false && a.IsSubmit == true)
                .ToListAsync();

            // Tạo danh sách học sinh và điểm số
            var studentGrades = classStudents
                .Where(cs => cs.UserId.HasValue)
                .Join(students,
                    cs => cs.UserId.Value,
                    s => s.Id,
                    (cs, s) => new { ClassStudent = cs, Student = s })
                .Where(joined => submittedAssignments.Any(a => a.UserId == joined.Student.Id))
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
                        : "Đã rời lớp",
                    SubmissionDate = assignments
                        .FirstOrDefault(a => a.UserId == joined.Student.Id && a.IsSubmit == true)?.SubmissionDate,
                    SubmissionDuration = CalculateSubmissionDuration(
                        test.StartDate,
                        assignments.FirstOrDefault(a => a.UserId == joined.Student.Id)?.SubmissionDate
                    )
                }).ToList();

            // Chuyển đổi Duration
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

            // Đọc nội dung đề tài từ file attachment
            string proposalContent = await ExtractProposalContentFromFileAsync(test.Attachment);

            // Tạo dữ liệu trả về
            var gradingData = new GradingDataResponse
            {
                TestId = test.Id,
                TestName = test.Topic ?? "Unknown",
                ClassId = selectedClassId,
                ClassName = classInfo.Name,
                Subject = test.Subject?.SubjectName ?? "Unknown",
                StartTime = test.StartDate ?? DateTimeOffset.MinValue,
                EndTime = test.EndDate ?? DateTimeOffset.MinValue,
                Duration = duration,
                Form = test.Form ?? "Unknown",
                Description = test.Description,
                Attachment = test.Attachment,
                IsExam = test.IsExam ?? false,
                ProposalContent = proposalContent,
                StudentGrades = studentGrades
            };

            return gradingData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy dữ liệu chấm điểm: {Message}. Stack trace: {StackTrace}", ex.Message,
                ex.StackTrace);

            // Log thêm thông tin về test đang xử lý
            if (test != null)
            {
                _logger.LogInformation(
                    "Thông tin test đang xử lý: ID={TestId}, SubjectId={SubjectId}, UserId={UserId}, ClassId={ClassId}",
                    test.Id, test.SubjectId, test.UserId, test.ClassId);
            }

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
                throw new Exception("Không tìm thấy thông tin giáo viên trong hệ thống");
            }

            // Kiểm tra bài kiểm tra hoặc lịch thi
            var test = await _context.TestExams
                .Include(t => t.Subject)
                .FirstOrDefaultAsync(t => t.Id == request.TestId && t.IsDelete == false);
            if (test == null)
            {
                throw new Exception("Không tìm thấy bài kiểm tra hoặc kỳ thi này trong hệ thống");
            }

            // Kiểm tra nếu UserId của bài thi là null
            if (test.UserId == null)
            {
                _logger.LogWarning("Bài thi ID: {TestId} không có thông tin người tạo (UserId là null)",
                    request.TestId);
                // Vẫn tiếp tục xử lý
            }
            else if (test.UserId == teacherId)
            {
                _logger.LogInformation("Giáo viên ID: {TeacherId} là người tạo bài thi ID: {TestId}", teacherId,
                    request.TestId);
                // Người tạo bài thi có quyền chấm điểm, tiếp tục xử lý
            }
            else
            {
                _logger.LogInformation(
                    "Kiểm tra bài thi ID: {TestId}, người tạo: {UserId}, giáo viên hiện tại: {TeacherId}",
                    request.TestId, test.UserId, teacherId);
            }

            // Lấy danh sách lớp liên kết với bài kiểm tra từ ClassTestExams
            var classTestExam = await _context.ClassTestExams
                .Where(cte => cte.TestExamId == request.TestId && cte.IsDelete == false)
                .ToListAsync();
            if (!classTestExam.Any())
            {
                throw new Exception("Bài kiểm tra hoặc kỳ thi này chưa được gán cho lớp học nào");
            }

            // Xử lý classId: từ ClassTestExams
            int selectedClassId;
            if (request.ClassId.HasValue)
            {
                if (!classTestExam.Any(cte => cte.ClassId == request.ClassId.Value))
                {
                    _logger.LogWarning(
                        $"ClassId={request.ClassId.Value} không tồn tại trong ClassTestExams của TestId={request.TestId}");
                    throw new Exception(
                        $"Lớp được chỉ định (ID: {request.ClassId.Value}) không liên kết với bài kiểm tra này!");
                }

                selectedClassId = request.ClassId.Value;
                _logger.LogInformation($"Sử dụng ClassId từ request: {selectedClassId}");
            }
            else
            {
                selectedClassId = classTestExam.First().ClassId.Value;
                _logger.LogInformation(
                    $"Không có ClassId trong request, sử dụng ClassId đầu tiên từ ClassTestExams: {selectedClassId}");
            }

            _logger.LogInformation($"Selected ClassId for TestId {request.TestId}: {selectedClassId}");

            // Kiểm tra thông tin lớp học
            var classInfo = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == selectedClassId && c.IsDelete == false);
            if (classInfo == null)
            {
                throw new Exception("Không tìm thấy thông tin lớp học trong hệ thống");
            }

            // Kiểm tra lớp có học môn của bài kiểm tra không
            var classSubject = await _context.ClassSubjects
                .FirstOrDefaultAsync(cs =>
                    cs.ClassId == selectedClassId && cs.SubjectId == test.SubjectId && cs.IsDelete == false);
            if (classSubject == null)
            {
                throw new Exception("Lớp học này không được gán môn học của bài kiểm tra hoặc kỳ thi");
            }

            // Kiểm tra quyền truy cập
            bool hasAccess = await HasGradingPermissionAsync(teacherId, request.TestId, test.IsExam ?? false);
            if (!hasAccess)
            {
                if (test.UserId != null && test.UserId != teacherId)
                {
                    throw new Exception(
                        "Giáo viên không có quyền chấm điểm bài thi này vì không phải người tạo bài thi!");
                }
                else
                {
                    throw new Exception("Giáo viên không có quyền chấm điểm bài thi này!");
                }
            }

            // Kiểm tra thời gian kết thúc bài kiểm tra
            var endDateUtc = test.EndDate.HasValue ? test.EndDate.Value.ToUniversalTime() : DateTimeOffset.MaxValue;
            _logger.LogInformation(
                $"TestId: {request.TestId}, EndDate (UTC): {endDateUtc}, UtcNow: {DateTimeOffset.UtcNow}");

            if (endDateUtc > DateTimeOffset.UtcNow)
            {
                _logger.LogWarning(
                    $"Bài kiểm tra TestId: {request.TestId} chưa kết thúc (EndDate: {endDateUtc}, Now: {DateTimeOffset.UtcNow})");
                throw new Exception("Bài kiểm tra hoặc kỳ thi này chưa kết thúc, không thể chấm điểm");
            }

            // Lấy danh sách học sinh hợp lệ trong lớp
            var classStudents = await _context.ClassStudents
                .Where(cs =>
                    cs.ClassId == selectedClassId && cs.IsActive == true && cs.IsDelete == false &&
                    cs.CreateAt <= test.StartDate)
                .ToListAsync();

            // Kiểm tra trạng thái học sinh trong bảng Users
            var studentIds = classStudents
                .Where(cs => cs.UserId.HasValue)
                .Select(cs => cs.UserId.Value)
                .ToList();
            var validStudents = await _context.Users
                .Where(u => studentIds.Contains(u.Id) && u.IsDelete == false)
                .Select(u => u.Id)
                .ToListAsync();

            // Ghi log danh sách học sinh hợp lệ
            _logger.LogInformation(
                $"Danh sách học sinh hợp lệ thuộc lớp ClassId: {selectedClassId} tại thời điểm bài kiểm tra TestId: {request.TestId}: {string.Join(", ", validStudents)}");

            // Kiểm tra học sinh có thuộc lớp không
            foreach (var grade in request.Grades)
            {
                if (!validStudents.Contains(grade.StudentId))
                {
                    throw new Exception(
                        "Một hoặc nhiều học sinh trong danh sách điểm không thuộc lớp học này hoặc đã bị xóa khỏi hệ thống");
                }

                // Kiểm tra điểm hợp lệ
                if (grade.Score.HasValue && (grade.Score < 0 || grade.Score > 10))
                {
                    throw new Exception(
                        "Điểm số của một hoặc nhiều học sinh không hợp lệ, điểm phải nằm trong khoảng từ 0 đến 10");
                }
            }

            // Lấy tất cả bài nộp cho bài kiểm tra này
            var assignments = await _context.Assignments
                .Where(a => a.TestExamId == request.TestId && a.IsDelete == false)
                .ToListAsync();

            // Ghi log để kiểm tra bài nộp
            if (!assignments.Any())
            {
                _logger.LogInformation($"Không tìm thấy bài nộp nào cho TestId: {request.TestId}");
            }
            else
            {
                foreach (var assignment in assignments)
                {
                    if (assignment.SubmissionFile != null &&
                        !assignment.SubmissionFile.StartsWith("https://res.cloudinary.com"))
                    {
                        _logger.LogWarning(
                            $"SubmissionFile của học sinh {assignment.UserId} cho TestId: {request.TestId} không phải URL từ Cloudinary: {assignment.SubmissionFile}");
                    }
                    else
                    {
                        _logger.LogInformation(
                            $"Bài nộp của học sinh {assignment.UserId} cho TestId: {request.TestId}, SubmissionFile: {assignment.SubmissionFile}");
                    }
                }
            }

            // Kiểm tra xem bài thi đã được chấm chưa
            if (assignments.Any(a => a.StatusAssignmentId == 3)) // Giả sử 3 là trạng thái đã chấm
            {
                throw new Exception("Bài kiểm tra hoặc kỳ thi này đã được chấm điểm, không thể chấm lại!");
            }

            // Lưu điểm cho từng học sinh
            foreach (var grade in request.Grades)
            {
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
                    assignment.StatusAssignmentId = 3;
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
            throw new Exception(ex.Message); // Truyền lỗi lên để hàm SaveGrades xử lý
        }
    }

    private async Task<bool> HasGradingPermissionAsync(int teacherId, int testId, bool isExam)
    {
        try
        {
            var teacher = await _context.Users.FindAsync(teacherId);
            if (teacher == null)
            {
                _logger.LogWarning("Không tìm thấy giáo viên với ID: {TeacherId}", teacherId);
                return false;
            }

            if (teacher.RoleId == 1)
            {
                _logger.LogInformation("Giáo viên ID: {TeacherId} là Admin, có toàn quyền", teacherId);
                return true; // Admin có toàn quyền
            }

            // Kiểm tra xem giáo viên có phải là người tạo bài thi không
            var test = await _context.TestExams
                .FirstOrDefaultAsync(t => t.Id == testId && t.IsDelete == false);

            if (test == null)
            {
                _logger.LogWarning("Không tìm thấy bài thi với ID: {TestId}", testId);
                return false;
            }

            // Kiểm tra nếu UserId của bài thi là null
            if (test.UserId == null)
            {
                _logger.LogWarning("Bài thi ID: {TestId} không có thông tin người tạo (UserId là null)", testId);
                // Vẫn tiếp tục kiểm tra các điều kiện khác
            }
            else
            {
                _logger.LogInformation(
                    "Kiểm tra bài thi ID: {TestId}, người tạo: {UserId}, giáo viên hiện tại: {TeacherId}",
                    testId, test.UserId, teacherId);

                // Nếu giáo viên là người tạo bài thi
                if (test.UserId == teacherId)
                {
                    _logger.LogInformation("Giáo viên ID: {TeacherId} là người tạo bài thi ID: {TestId}", teacherId,
                        testId);
                    return true;
                }
            }

            // Kiểm tra quyền truy cập vào lớp học từ class_test_exams
            var classTestExam = await _context.ClassTestExams
                .Where(cte => cte.TestExamId == testId && cte.IsDelete == false)
                .ToListAsync();

            if (!classTestExam.Any())
            {
                _logger.LogWarning("Không tìm thấy bản ghi class_test_exam nào cho TestId: {TestId}", testId);
                return false;
            }

            foreach (var cte in classTestExam)
            {
                // Kiểm tra giáo viên chủ nhiệm
                var classInfo = await _context.Classes
                    .FirstOrDefaultAsync(c => c.Id == cte.ClassId && c.IsDelete == false);
                if (classInfo == null)

                {
                    _logger.LogWarning("Không tìm thấy thông tin lớp với ID: {ClassId}", cte.ClassId);
                    continue;
                }

                if (classInfo.UserId == teacherId)
                {
                    _logger.LogInformation("Giáo viên ID: {TeacherId} là chủ nhiệm lớp ID: {ClassId}", teacherId,
                        cte.ClassId);
                    return true; // Giáo viên chủ nhiệm
                }

                // Kiểm tra giáo viên được phân công giảng dạy môn học
                if (test.SubjectId.HasValue)
                {
                    var teacherAssignment = await _context.TeachingAssignments
                        .FirstOrDefaultAsync(ta =>
                            ta.ClassId == cte.ClassId &&
                            ta.SubjectId == test.SubjectId &&
                            ta.UserId == teacherId &&
                            ta.IsDelete == false);

                    if (teacherAssignment != null)
                    {
                        _logger.LogInformation(
                            "Giáo viên ID: {TeacherId} được phân công dạy môn ID: {SubjectId} cho lớp ID: {ClassId}",
                            teacherId, test.SubjectId, cte.ClassId);
                        return true; // Giáo viên được phân công giảng dạy
                    }
                }
            }

            _logger.LogWarning("Giáo viên ID: {TeacherId} không có quyền chấm điểm cho bài thi ID: {TestId}", teacherId,
                testId);
            return false; // Nếu không thuộc các trường hợp trên thì không có quyền
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi kiểm tra quyền chấm điểm: {Message}", ex.Message);
            return false;
        }
    }

    private async Task<string> ExtractProposalContentFromFileAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl))
        {
            return "Đường dẫn file không hợp lệ, vui lòng kiểm tra lại.";
        }

        var fileExtension = Path.GetExtension(fileUrl).ToLower();
        try
        {
            // Kiểm tra định dạng file dựa trên phần mở rộng
            if (fileExtension != ".doc" && fileExtension != ".docx" && fileExtension != ".xlsx" &&
                fileExtension != ".xls")
            {
                _logger.LogWarning($"Chỉ hỗ trợ định dạng .doc, .docx, .xlsx, và .xls: {fileUrl}");
                return "Chỉ hỗ trợ định dạng file Word (.doc, .docx) hoặc Excel (.xlsx, .xls).";
            }

            using (var httpClient = new HttpClient())
            {
                // Lấy byte array từ URL của file
                byte[] fileBytes = await httpClient.GetByteArrayAsync(fileUrl);

                // Xử lý file Word (.doc, .docx)
                if (fileExtension == ".doc" || fileExtension == ".docx")
                {
                    using (MemoryStream stream = new MemoryStream(fileBytes))
                    {
                        Document document = new Document(stream);
                        string content = document.GetText();
                        if (string.IsNullOrEmpty(content))
                        {
                            return "Không có nội dung trong file Word để trích xuất.";
                        }

                        return ExtractProposalTitle(content);
                    }
                }
                // Xử lý file Excel (.xlsx, .xls)
                else if (fileExtension == ".xlsx" || fileExtension == ".xls")
                {
                    using (MemoryStream stream = new MemoryStream(fileBytes))
                    {
                        using (var package = new ExcelPackage(stream))
                        {
                            // Lấy sheet đầu tiên
                            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                            if (worksheet == null)
                            {
                                _logger.LogWarning($"File Excel không có sheet nào: {fileUrl}");
                                return "File Excel không có dữ liệu, vui lòng kiểm tra lại.";
                            }

                            // Đọc nội dung từ các ô trong sheet
                            StringBuilder contentBuilder = new StringBuilder();
                            int rowCount = worksheet.Dimension?.Rows ?? 0;
                            int colCount = worksheet.Dimension?.Columns ?? 0;

                            if (rowCount == 0 || colCount == 0)
                            {
                                _logger.LogWarning($"File Excel không có dữ liệu: {fileUrl}");
                                return "File Excel không có dữ liệu, vui lòng kiểm tra lại.";
                            }

                            // Duyệt qua từng ô trong sheet và ghép nội dung
                            for (int row = 1; row <= rowCount; row++)
                            {
                                for (int col = 1; col <= colCount; col++)
                                {
                                    var cellValue = worksheet.Cells[row, col].Text?.Trim();
                                    if (!string.IsNullOrEmpty(cellValue))
                                    {
                                        contentBuilder.AppendLine(cellValue);
                                    }
                                }
                            }

                            string content = contentBuilder.ToString();
                            if (string.IsNullOrEmpty(content))
                            {
                                return "Không có nội dung trong file Excel để trích xuất.";
                            }

                            return ExtractProposalTitle(content);
                        }
                    }
                }

                // Trường hợp không xác định (dự phòng)
                return "Không thể xác định định dạng file, vui lòng kiểm tra lại.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi đọc nội dung file từ URL {fileUrl}: {ex.Message}");
            if (fileExtension == ".doc" || fileExtension == ".docx")
            {
                return "Không thể đọc nội dung file Word, vui lòng thử lại sau.";
            }
            else if (fileExtension == ".xlsx" || fileExtension == ".xls")
            {
                return "Không thể đọc nội dung file Excel, vui lòng thử lại sau.";
            }

            return "Không thể đọc nội dung file, vui lòng thử lại sau.";
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
                    return "Không tìm thấy tên đề bài trong file.";
                }

                return proposalTitle;
            }
        }

        // Nếu không tìm thấy dòng "Đề bài"
        return "Không tìm thấy dòng 'Đề bài' trong file.";
    }

    private string CalculateSubmissionDuration(DateTimeOffset? testStartTime, DateTimeOffset? submissionTime)
    {
        if (!testStartTime.HasValue || !submissionTime.HasValue)
            return "N/A";

        // So sánh trực tiếp DateTimeOffset mà không chuyển đổi
        if (submissionTime.Value < testStartTime.Value)
        {
            _logger.LogWarning(
                $"Thời gian nộp bài ({submissionTime}) sớm hơn thời gian bắt đầu ({testStartTime}). Vui lòng kiểm tra dữ liệu.");
            return "0 giây";
        }

        TimeSpan duration = submissionTime.Value - testStartTime.Value;

        if (duration.TotalDays >= 1)
        {
            int days = (int)Math.Floor(duration.TotalDays);
            int hours = duration.Hours;
            return $"{days} ngày {hours} giờ {duration.Minutes} phút";
        }
        else if (duration.TotalHours >= 1)
        {
            return $"{Math.Floor(duration.TotalHours)} giờ {duration.Minutes} phút";
        }
        else if (duration.TotalMinutes >= 1)
        {
            return $"{duration.Minutes} phút {duration.Seconds} giây";
        }
        else
        {
            return $"{duration.Seconds} giây";
        }
    }
}