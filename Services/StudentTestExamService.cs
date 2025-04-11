using System.Text.RegularExpressions;
using AutoMapper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlAgilityPack;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Hubs;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;


namespace Project_LMS.Services;

public class StudentTestExamService : IStudentTestExamService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<RealtimeHub> _hubContext;

    public StudentTestExamService(ApplicationDbContext context, IMapper mapper, IServiceProvider serviceProvider,
        IHubContext<RealtimeHub> hubContext)
    {
        _context = context;
        _mapper = mapper;
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
    }


    public async Task<ApiResponse<PaginatedResponse<StudentTestExamResponse>>> GetStudentTestExamAsync(
        int studentId, int? pageNumber, int? pageSize, string? sortDirection,
        string? topicName, int? subjectId, int? departmentId, string? startDate, string? option)
    {
        var today = DateTime.Now.Date;

        var testExamQuery = _context.TestExams
            .Where(te => te.IsDelete == false && te.IsExam ==false
                         && te.ClassTestExams.Any(cte =>
                             cte.Class.ClassStudents.Any(sc => sc.UserId == studentId)));

        // Cập nhật trạng thái bài kiểm tra
        var testExamsToUpdate = await testExamQuery.ToListAsync();
        foreach (var testExam in testExamsToUpdate)
        {
            // Cập nhật trạng thái của bài kiểm tra
            if (testExam.EndDate.Value <= today)
            {
                testExam.ScheduleStatusId = 7; // Đã kết thúc
            }
            else if (testExam.StartDate.Value == today)
            {
                testExam.ScheduleStatusId = 6; // Đang diễn ra
            }
            else if (testExam.StartDate.Value < today && testExam.EndDate.Value >= today)
            {
                testExam.ScheduleStatusId = 3; // Đang diễn ra
            }
            else if (testExam.StartDate.Value == testExam.EndDate.Value)
            {
                if (testExam.StartDate.Value < today)
                {
                    testExam.ScheduleStatusId = 7; // Đã kết thúc
                }
            }
            _context.TestExams.Update(testExam);
        }

        // Lưu các thay đổi
        await _context.SaveChangesAsync();
        
        // Kiểm tra đầu vào
        if (pageNumber.HasValue && pageNumber <= 0)
        {
            return new ApiResponse<PaginatedResponse<StudentTestExamResponse>>(
                1, "Giá trị pageNumber phải lớn hơn 0", null);
        }

        if (pageSize.HasValue && pageSize <= 0)
        {
            return new ApiResponse<PaginatedResponse<StudentTestExamResponse>>(
                1, "Giá trị pageSize phải lớn hơn 0", null);
        }

        if (!string.IsNullOrEmpty(sortDirection) &&
            !sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase) &&
            !sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
        {
            return new ApiResponse<PaginatedResponse<StudentTestExamResponse>>(
                1, "Giá trị sortDirection phải là 'asc' hoặc 'desc'", null);
        }

        // Truy vấn cơ sở dữ liệu
        // var testExamQuery = _context.TestExams
        //     .Where(te => te.IsDelete == false && te.IsExam == false
        //                                       && te.ClassTestExams.Any(cte =>
        //                                           cte.Class.ClassStudents.Any(sc => sc.UserId == studentId)));

        
        // Truy vấn cơ sở dữ liệu
        // var testExamQuery = _context.TestExams
        //     .Where(te => te.IsDelete == false 
        //                                       && te.ClassTestExams.Any(cte =>
        //                                           cte.Class.ClassStudents.Any(sc => sc.UserId == studentId)));
        //
        // Các điều kiện lọc
        if (!string.IsNullOrEmpty(topicName))
        {
            var decodedTopicName = Uri.UnescapeDataString(topicName);
            testExamQuery = testExamQuery.Where(te => EF.Functions.Like(te.Topic, $"%{decodedTopicName}%"));
        }

        if (subjectId.HasValue)
        {
            testExamQuery = testExamQuery.Where(te => te.Subject.Id == subjectId.Value);
        }

        if (departmentId.HasValue)
        {
            testExamQuery =  testExamQuery.Where(te => te.Department.Id == departmentId.Value);
        }

        if (!string.IsNullOrEmpty(startDate) && DateTime.TryParseExact(startDate, "dd-MM-yyyy",
                new System.Globalization.CultureInfo("vi-VN"), System.Globalization.DateTimeStyles.None,
                out var parsedDate))
        {
            testExamQuery = testExamQuery.Where(te =>
                te.StartDate.HasValue && te.StartDate.Value.Date == parsedDate.Date);
        }

        if (!string.IsNullOrEmpty(option))
        {
            if (option.Equals("completed", StringComparison.OrdinalIgnoreCase))
            {
                // Filtering completed exams based on the endDate and ScheduleStatusId
                testExamQuery = testExamQuery.Where(te =>
                    te.EndDate.HasValue && te.EndDate.Value < DateTime.Now);
            }
            else if (option.Equals("incomplete", StringComparison.OrdinalIgnoreCase))
            {
                // Filtering incomplete exams based on the startDate
                testExamQuery = testExamQuery.Where(te =>
                    te.StartDate.HasValue && te.StartDate.Value > DateTime.Now);
            }
        }


        // Đảm bảo chỉ chọn dữ liệu cần thiết
        testExamQuery = testExamQuery
            .Include(te => te.Subject)
            .Include(te => te.ExamScheduleStatus)
            .Include(te => te.ClassTestExams)
            .ThenInclude(cte => cte.Class)
            .ThenInclude(cl => cl.User);

        // Sắp xếp theo ngày bắt đầu
        sortDirection ??= "asc";
        testExamQuery = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
            ? testExamQuery.OrderByDescending(te => te.StartDate)
            : testExamQuery.OrderBy(te => te.StartDate);

        // Phân trang
        var currentPage = pageNumber ?? 1;
        var currentPageSize = pageSize ?? 10;
        var totalItems = await testExamQuery.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / currentPageSize);

        var pagedTestExams = await testExamQuery
            .Skip((currentPage - 1) * currentPageSize)
            .Take(currentPageSize)
            .ToListAsync();

        var mappedData = _mapper.Map<List<StudentTestExamResponse>>(pagedTestExams);

        var paginatedResponse = new PaginatedResponse<StudentTestExamResponse>
        {
            Items = mappedData,
            PageNumber = currentPage,
            PageSize = currentPageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasPreviousPage = currentPage > 1,
            HasNextPage = currentPage < totalPages
        };

        return new ApiResponse<PaginatedResponse<StudentTestExamResponse>>(
            0, "Lấy dữ liệu thành công", paginatedResponse);
    }

    public async Task<ApiResponse<List<QuestionResponse>>> GetStudentTestExamByIdAsync(int id, int userId)
    {
        // Lấy bài kiểm tra theo id, bao gồm các câu hỏi (và câu trả lời nếu có) và thông tin môn học
        var testExam = await _context.TestExams
            .Include(te => te.Questions)
            .ThenInclude(q => q.Answers)
            .Include(te => te.Subject)
            .Include(te => te.ClassTestExams)
            .ThenInclude(cte => cte.Class)
            .FirstOrDefaultAsync(te => te.Id == id);

        if (testExam == null)
        {
            return new ApiResponse<List<QuestionResponse>>(1, "Không tìm thấy bài kiểm tra", null);
        }
        
        if (testExam.EndDate.HasValue && DateTime.Now > testExam.EndDate.Value)
        {
            return new ApiResponse<List<QuestionResponse>>(1, "Bài kiểm tra đã kết thúc, không thể truy cập!", null);
        }
        
        // Lọc các câu hỏi dựa trên hình thức của bài kiểm tra
        List<Question> filteredQuestions;
        if (testExam.Form.IndexOf("trắc nghiệm", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            filteredQuestions = testExam.Questions
                .Where(q => q.QuestionType == "trắc nghiệm")
                .ToList();
        }
        else
        {
            // Giả sử các hình thức còn lại là tự luận (hoặc có thể xử lý các trường hợp khác nếu cần)
            filteredQuestions = testExam.Questions
                .Where(q => q.QuestionType == "tự luận")
                .ToList();
        }

        if (filteredQuestions == null || !filteredQuestions.Any())
        {
            return new ApiResponse<List<QuestionResponse>>(1, "Không có câu hỏi phù hợp trong bài kiểm tra", null);
        }

        var className = await _context.Users
            .Where(u => u.Id == userId)
            .Include(u => u.ClassStudents)
            .ThenInclude(cs => cs.Class)
            .Select(u => u.ClassStudents
                .Select(cs => cs.Class.Name)
                .FirstOrDefault())
            .FirstOrDefaultAsync();

        // Xử lý thời gian (duration)
        var durationTimeSpan = testExam.Duration.Value.ToTimeSpan(); // Chuyển TimeOnly thành TimeSpan
        var hours = durationTimeSpan.Hours;
        var minutes = durationTimeSpan.Minutes;

        string durationString;
        if (hours > 0 && minutes > 0)
        {
            durationString = $"{hours} giờ {minutes} phút";
        }
        else if (hours > 0)
        {
            durationString = $"{hours} giờ";
        }
        else
        {
            durationString = $"{minutes} phút";
        }

        // Random thứ tự câu hỏi và mapping sang QuestionResponse
        var randomQuestions = filteredQuestions
            .OrderBy(_ => Guid.NewGuid()) // Xáo trộn thứ tự
            .Select((q, index) =>
            {
                var questionResponse = new QuestionResponse
                {
                    QuestionNumber = $"Câu {index + 1}",
                    QuestionId = q.Id,
                    Question = q.QuestionText,
                    Duration = durationString,
                    SubjectName = testExam.Subject.SubjectName,
                    ClassName = className,
                    StartDate = testExam.StartDate.HasValue
                        ? testExam.StartDate.Value.ToString("dddd, 'ngày' dd-MM-yyyy, HH:mm",
                            new System.Globalization.CultureInfo("vi-VN"))
                        : string.Empty,
                };

                if (q.QuestionType == "trắc nghiệm")
                {
                    // Với trắc nghiệm: fill danh sách câu trả lời
                    questionResponse.Answers = q.Answers?
                        .Select(a => new AnswerResponseByQuestionId
                        {
                            AnswerId = a.Id,
                            AnswerText = a.Answer1
                        }).ToList() ?? new List<AnswerResponseByQuestionId>();
                }
                else if (q.QuestionType == "tự luận")
                {
                    // Với tự luận: không có danh sách câu trả lời
                    questionResponse.Answers = null;
                }

                return questionResponse;
            })
            .ToList();

        return new ApiResponse<List<QuestionResponse>>(0, "Lấy dữ liệu thành công", randomQuestions);
    }

    public async Task<ApiResponse<object>> SubmitYourAssignment(int UserId, SubmitMultipleChoiceQuestionRequest request)
    {
        var userClasses = await _context.ClassStudents
            .Where(cls => cls.UserId == UserId)
            .Select(cls => cls.ClassId)
            .ToListAsync();

        var examClasses = await _context.ClassTestExams
            .Where(exam => exam.TestExamId == request.TestExamId)
            .Select(exam => exam.ClassId)
            .ToListAsync();

        if (!userClasses.Intersect(examClasses).Any())
        {
            return new ApiResponse<object>(1, $"User {UserId} không có bài kiểm tra {request.TestExamId}", null);
        }

        // ✅ Kiểm tra thời gian hợp lệ
        var testExam = await _context.TestExams.FirstOrDefaultAsync(ts => ts.Id == request.TestExamId);
        if (testExam == null) return new ApiResponse<object>(1, "Bài kiểm tra không tồn tại", null);
        if (testExam.StartDate.HasValue && DateTime.Now < testExam.StartDate.Value)
            return new ApiResponse<object>(1, "Chưa đến thời gian làm bài", null);
        if (testExam.EndDate.HasValue && DateTime.Now > testExam.EndDate.Value)
            return new ApiResponse<object>(1, "Hết thời gian làm bài", null);



        // Create a new Assignment first
        var assignment = new Assignment
        {
            UserId = UserId,
            TestExamId = request.TestExamId,
            IsSubmit = true,
            SubmissionDate = DateTime.Now,
        };

        _context.Assignments.Add(assignment);
        await _context.SaveChangesAsync(); // Save the assignment first to generate its Id

        // Retrieve all questions for the TestExamId
        var questions = await _context.Questions
            .Where(q => q.TestExamId == request.TestExamId)
            .ToListAsync();

        if (questions.Count == 0)
        {
            return new ApiResponse<object>(1, "Không có câu hỏi trong bài kiểm tra!", null);
        }

        double totalScore = 0;
           int correctCount = 0;
           int incorrectCount = 0;
           int totalQuestions = questions.Count;
           var reviewList = new List<object>();
           foreach (var answerId in request.AnswerIds)
           {
               var answer = await _context.Answers
                   .Include(a => a.Question)
                   .FirstOrDefaultAsync(a => a.Id == answerId);
               bool isCorrect = answer.IsCorrect ?? false;
               if (answer != null && answer.Question != null)
               {
               
       
                   if (isCorrect)
                   {
                       totalScore += answer.Question.Mark ?? 0;
                       correctCount++;
                   }
                   else
                   {
                       incorrectCount++;
                   }
       
                   _context.AssignmentDetails.Add(new AssignmentDetail
                   {
                       AssignmentId = assignment.Id,
                       AnswerId = answerId,
                       IsCorrect = isCorrect,
                       CreateAt = DateTime.Now,
                       UpdateAt = DateTime.Now
                   });
                   
               }
               reviewList.Add(new
               {
                   QuestionId = answer.Question.Id,
                   QuestionContent = answer.Question.QuestionText,
                   SelectedAnswerId = answer.Id,
                   SelectedAnswerContent = answer.Answer1,
                   IsCorrect =  isCorrect
               });
           }
       
           assignment.TotalScore = totalScore;
       
           await _context.SaveChangesAsync();
       
           return new ApiResponse<object>(0, "Nộp bài thành công!", new
           {
               TotalQuestions = totalQuestions,
               TotalScore = totalScore,
               CorrectCount = correctCount,
               IncorrectCount = incorrectCount,
               Review = reviewList
               
           });
    }
public async Task<ApiResponse<object>> SaveEssay(int UserId, SaveEssayRequest request)
{
    // ✅ Kiểm tra User có bài kiểm tra không
    var userClasses = await _context.ClassStudents
        .Where(cls => cls.UserId == UserId)
        .Select(cls => cls.ClassId)
        .ToListAsync();

    var examClasses = await _context.ClassTestExams
        .Where(exam => exam.TestExamId == request.TestExamId)
        .Select(exam => exam.ClassId)
        .ToListAsync();

    if (!userClasses.Intersect(examClasses).Any())
    {
        return new ApiResponse<object>(1, $"User {UserId} không có bài kiểm tra {request.TestExamId}", null);
    }

    // ✅ Kiểm tra thời gian hợp lệ
    var testExam = await _context.TestExams.FirstOrDefaultAsync(ts => ts.Id == request.TestExamId);
    if (testExam == null) return new ApiResponse<object>(1, "Bài kiểm tra không tồn tại", null);
    
    var nowUtc = DateTime.UtcNow;
    if (testExam.StartDate.HasValue && nowUtc < testExam.StartDate.Value.ToUniversalTime())
        return new ApiResponse<object>(1, "Chưa đến thời gian làm bài", null);

    if (testExam.EndDate.HasValue && nowUtc > testExam.EndDate.Value.ToUniversalTime())
        return new ApiResponse<object>(1, "Hết thời gian làm bài", null);

    // ✅ Kiểm tra xem User đã có bài nộp chưa
    var existingAssignment = await _context.Assignments
        .FirstOrDefaultAsync(a => a.UserId == UserId && a.TestExamId == request.TestExamId);

    string fileUrl = await SaveEssayToFile(request.SubmissionFile);

    if (existingAssignment == null)
    {
        existingAssignment = new Assignment
        {
            UserId = UserId,
            SubmissionFile = fileUrl,
            TestExamId = request.TestExamId,
            SubmissionDate = DateTime.Now,
            IsSubmit = true
        };
        await _context.Assignments.AddAsync(existingAssignment);
    }
    else
    {
        // Cập nhật lại các thông tin cần thiết
        existingAssignment.SubmissionFile = fileUrl;
        existingAssignment.SubmissionDate = DateTime.Now;
        existingAssignment.IsSubmit = true;

        _context.Assignments.Update(existingAssignment);
    }

    await _context.SaveChangesAsync();


    // ✅ Lấy danh sách định dạng file được phép
    var fileFormats = await _context.FileFormats
        .Where(ts => ts.TestExamId == request.TestExamId)
        .ToListAsync();

    if (!fileFormats.Any())
    {
        return new ApiResponse<object>(1, "Không có định dạng file nào được phép!", null);
    }

    bool isDocAllowed = fileFormats.Any(f => f.IsDoc == true);
    bool isPptAllowed = fileFormats.Any(f => f.IsPowerpoint == true);
    bool isXlsAllowed = fileFormats.Any(f => f.IsXxls == true);
    bool isJpegAllowed = fileFormats.Any(f => f.IsJpeg == true);

    // ✅ Tính dung lượng tối đa cho phép
    long maxSize = fileFormats.Max(f =>
        (f.Is10 == true ? 10 : 0) +
        (f.Is20 == true ? 20 : 0) +
        (f.Is30 == true ? 30 : 0) +
        (f.Is40 == true ? 40 : 0)) * 1024 * 1024;


    var cloudinaryService = _serviceProvider.GetService<ICloudinaryService>();
    var uploadedFiles = new List<SubmissionFile>();

    foreach (var file in request.AttachedFiles)
    {
        // ✅ Kiểm tra dung lượng file
        string base64Data = Regex.Match(file, @"base64,(?<data>.*)").Groups["data"].Value;
        if (string.IsNullOrEmpty(base64Data)) continue;

        byte[] fileBytes = Convert.FromBase64String(base64Data);
        if (fileBytes.Length > maxSize)
        {
            return new ApiResponse<object>(1, $"File {file.Substring(0, 30)}... vượt quá dung lượng cho phép!", null);
        }

        // ✅ Xác định loại file và upload
        string uploadResult = null;
        if (file.Contains("application/msword") && isDocAllowed)
        {
            uploadResult = await cloudinaryService.UploadDocxAsync(file);
        }
        else if (file.Contains("application/vnd.ms-powerpoint") && isPptAllowed)
        {
            uploadResult = await cloudinaryService.UploadPowerPointAsync(file);
        }
        else if (file.Contains("application/vnd.ms-excel") && isXlsAllowed)
        {
            uploadResult = await cloudinaryService.UploadExcelAsync(file);
        }
        else if (file.Contains("image/jpeg") && isJpegAllowed)
        {
            uploadResult = await cloudinaryService.UploadImageAsync(file);
        }
        else
        {
            return new ApiResponse<object>(1, $"File {file.Substring(0, 30)}... có định dạng không hợp lệ!", null);
        }

        if (string.IsNullOrEmpty(uploadResult))
        {
            return new ApiResponse<object>(1, $"Lỗi khi tải file {file.Substring(0, 30)}... lên Cloudinary!", null);
        }

        // ✅ Lưu file vào SubmissionFiles
        uploadedFiles.Add(new SubmissionFile
        {
            AssignmentId = existingAssignment.Id,
            FileName = uploadResult
        });
    }

    if (uploadedFiles.Any())
    {
        await _context.SubmissionFiles.AddRangeAsync(uploadedFiles);
        await _context.SaveChangesAsync();
    }

    return new ApiResponse<object>(0, "Nộp bài thành công!", null);
}


//     public async Task<ApiResponse<object>> SaveEssay(int UserId, SaveEssayRequest request)
//     {
//         var userClasses = await _context.ClassStudents
//             .Where(cls => cls.UserId == UserId)
//             .Select(cls => cls.ClassId)
//             .ToListAsync();
//
//         var examClasses = await _context.ClassTestExams
//             .Where(exam => exam.TestExamId == request.TestExamId)
//             .Select(exam => exam.ClassId)
//             .ToListAsync();
//
//         bool isInClassWithExam = userClasses.Intersect(examClasses).Any();
//         if (!isInClassWithExam)
//         {
//             return new ApiResponse<object>(1,
//                 $"User {UserId} không có bài kiểm tra {request.TestExamId} trong các lớp {string.Join(", ", userClasses)}",
//                 null);
//         }
//
//         var testExam = await _context.TestExams.FirstOrDefaultAsync(ts => ts.Id == request.TestExamId);
//         if (testExam == null) return new ApiResponse<object>(1, "Bài kiểm tra không tồn tại", null);
//         if (testExam.StartDate.HasValue && DateTime.Now < testExam.StartDate.Value)
//             return new ApiResponse<object>(1, "Chưa đến thời gian làm bài", null);
//         if (testExam.EndDate.HasValue && DateTime.Now > testExam.EndDate.Value)
//             return new ApiResponse<object>(1, "Hết thời gian làm bài", null);
//
//         // Kiểm tra xem đã có bài nộp chưa
//         var existingAssignment = await _context.Assignments
//             .FirstOrDefaultAsync(a => a.UserId == UserId && a.TestExamId == request.TestExamId);
//
//         string fileUrl = await SaveEssayToFile(request.SubmissionFile);
//
//         if (existingAssignment != null)
//         {
//             existingAssignment.SubmissionFile = fileUrl;
//             _context.Assignments.Update(existingAssignment);
//             await _context.SaveChangesAsync();
//         }
//         else
//         {
//             var assignment = new Assignment
//             {
//                 UserId = UserId,
//                 TestExamId = request.TestExamId,
//                 SubmissionFile = fileUrl,
//                 SubmissionDate = DateTime.Now,
//                 IsSubmit = true
//             };
//             await _context.Assignments.AddAsync(assignment);
//             await _context.SaveChangesAsync();
//
//             var fileFormats = await _context.FileFormats
//                 .Where(ts => ts.TestExamId == request.TestExamId)
//                 .ToListAsync();
//
//
//             if (!fileFormats.Any())
//             {
//                 return new ApiResponse<object>(1, "Không có định dạng file nào được phép!", null);
//             }
//
//
//             bool isDocAllowed = fileFormats.Any(f => f.IsDoc == true);
//             bool isPptAllowed = fileFormats.Any(f => f.IsPowerpoint == true);
//             bool isXlsAllowed = fileFormats.Any(f => f.IsXxls == true);
//             bool isJpegAllowed = fileFormats.Any(f => f.IsJpeg == true);
//
//
//             long maxSize = fileFormats.Max(f =>
//                 (f.Is10 == true ? 10 : 0) +
//                 (f.Is20 == true ? 20 : 0) +
//                 (f.Is30 == true ? 30 : 0) +
//                 (f.Is40 == true ? 40 : 0)) * 1024 * 1024;
//
// // Kiểm tra file định dạng trước khi upload
//             string uploadResult = null;
//             long fileSize = 0;
//
// // Trích xuất dữ liệu từ base64
//             string base64Data = Regex.Match(request.AttachedFile, @"base64,(?<data>.*)").Groups["data"].Value;
//             if (!string.IsNullOrEmpty(base64Data))
//             {
//                 byte[] fileBytes = Convert.FromBase64String(base64Data);
//                 fileSize = fileBytes.Length;
//             }
//
// // Kiểm tra dung lượng file
//             if (fileSize > maxSize)
//             {
//                 return new ApiResponse<object>(1,
//                     $"File vượt quá dung lượng cho phép! Tối đa {maxSize / (1024 * 1024)}MB.", null);
//             }
//
// // Xác định đúng loại file và upload
//             var cloudinaryService = _serviceProvider.GetService<ICloudinaryService>();
//
//             if (request.AttachedFile.Contains("application/msword") && isDocAllowed)
//             {
//                 uploadResult = await cloudinaryService.UploadDocxAsync(request.AttachedFile);
//             }
//             else if (request.AttachedFile.Contains("application/vnd.ms-powerpoint") && isPptAllowed)
//             {
//                 uploadResult = await cloudinaryService.UploadPowerPointAsync(request.AttachedFile);
//             }
//             else if (request.AttachedFile.Contains("application/vnd.ms-excel") && isXlsAllowed)
//             {
//                 uploadResult = await cloudinaryService.UploadExcelAsync(request.AttachedFile);
//             }
//             else if (request.AttachedFile.Contains("image/jpeg") && isJpegAllowed)
//             {
//                 uploadResult = await cloudinaryService.UploadImageAsync(request.AttachedFile);
//             }
//             else
//             {
//                 return new ApiResponse<object>(1, "Loại file không hợp lệ!", null);
//             }
//
// // Kiểm tra upload thành công chưa
//             if (string.IsNullOrEmpty(uploadResult))
//             {
//                 return new ApiResponse<object>(1, "Lỗi khi tải file lên Cloudinary!", null);
//             }
//
// // Thêm hoặc cập nhật SubmissionFile
//             var assignmentId = existingAssignment?.Id ?? await _context.Assignments
//                 .Where(a => a.UserId == UserId && a.TestExamId == request.TestExamId)
//                 .Select(a => a.Id)
//                 .FirstOrDefaultAsync();
//
//             var existingSubmissionFile = await _context.SubmissionFiles
//                 .FirstOrDefaultAsync(sf => sf.AssignmentId == assignmentId);
//
//             if (existingSubmissionFile != null)
//             {
//                 Console.WriteLine("Debug: Đã tìm thấy SubmissionFile -> Tiến hành update");
//                 existingSubmissionFile.FileName = uploadResult;
//                 _context.SubmissionFiles.Update(existingSubmissionFile);
//             }
//             else
//             {
//                 Console.WriteLine("Debug: Không tìm thấy SubmissionFile -> Tạo mới");
//                 var submissionFile = new SubmissionFile
//                 {
//                     AssignmentId = assignmentId,
//                     FileName = uploadResult
//                 };
//                 await _context.SubmissionFiles.AddAsync(submissionFile);
//             }
//         }
//
//         await _context.SaveChangesAsync();
//         return new ApiResponse<object>(0, "Nộp bài thành công!", null);
//     }


    public async Task<string> SaveEssayToFile(string content)
    {
        string filePath = Path.Combine(Path.GetTempPath(), "abc.docx");

        using (WordprocessingDocument wordDocument =
               WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
        {
            MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
            mainPart.Document = new Document();
            Body body = new Body();

            // Phân tích HTML
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(content);

            // Duyệt qua từng phần tử HTML
            foreach (var node in htmlDoc.DocumentNode.ChildNodes)
            {
                if (node.Name == "p") // Chỉ xử lý đoạn văn bản <p>
                {
                    Paragraph paragraph = new Paragraph();
                    ParagraphProperties paragraphProperties = new ParagraphProperties();

                    // Kiểm tra căn lề trong style (text-align)
                    string style = node.GetAttributeValue("style", "").ToLower();
                    if (style.Contains("text-align: center"))
                    {
                        paragraphProperties.Justification = new Justification() { Val = JustificationValues.Center };
                    }
                    else if (style.Contains("text-align: right"))
                    {
                        paragraphProperties.Justification = new Justification() { Val = JustificationValues.Right };
                    }
                    else
                    {
                        paragraphProperties.Justification =
                            new Justification() { Val = JustificationValues.Left }; // Mặc định là căn trái
                    }

                    paragraph.Append(paragraphProperties);

                    // Xử lý các định dạng như in đậm, in nghiêng, gạch chân
                    foreach (var childNode in node.ChildNodes)
                    {
                        Run run = new Run();
                        RunProperties runProperties = new RunProperties();

                        if (childNode.Name == "b" || childNode.Name == "strong")
                        {
                            runProperties.Append(new Bold());
                        }

                        if (childNode.Name == "i" || childNode.Name == "em")
                        {
                            runProperties.Append(new Italic());
                        }

                        if (childNode.Name == "u")
                        {
                            runProperties.Append(new Underline() { Val = UnderlineValues.Single });
                        }

                        run.Append(runProperties);
                        run.Append(new Text(childNode.InnerText));
                        paragraph.Append(run);
                    }

                    body.Append(paragraph);
                }
            }

            mainPart.Document.Append(body);
            mainPart.Document.Save();
        }

        // Chuyển file DOCX sang base64 để upload lên Cloudinary
        byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
        string base64String = Convert.ToBase64String(fileBytes);
        var cloudinaryService = _serviceProvider.GetService<ICloudinaryService>();

        // Gửi file lên Cloudinary và nhận URL
        string fileUrl = await cloudinaryService.UploadDocxAsync(base64String);

        // Xóa file tạm
        File.Delete(filePath);

        return fileUrl;
    }
}