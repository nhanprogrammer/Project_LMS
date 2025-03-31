using System.Text.RegularExpressions;
using AutoMapper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
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

    public StudentTestExamService(ApplicationDbContext context, IMapper mapper, IServiceProvider serviceProvider, IHubContext<RealtimeHub> hubContext)
    {
        _context = context;
        _mapper = mapper;
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
    }


    public async Task<ApiResponse<PaginatedResponse<StudentTestExamResponse>>> GetStudentTestExamAsync(
        int studentId, int? pageNumber, int? pageSize, string? sortDirection,
        string? topicName, string? subjectName, string? department, string? startDate, string? option)
    {
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
        var testExamQuery = _context.TestExams
            .Where(te => te.IsDelete == false && te.IsExam == false
                                              && te.ClassTestExams.Any(cte =>
                                                  cte.Class.ClassStudents.Any(sc => sc.UserId == studentId)));

        // Các điều kiện lọc
        if (!string.IsNullOrEmpty(topicName))
        {
            var decodedTopicName = Uri.UnescapeDataString(topicName);
            testExamQuery = testExamQuery.Where(te => EF.Functions.Like(te.Topic, $"%{decodedTopicName}%"));
        }

        if (!string.IsNullOrEmpty(subjectName))
        {
            testExamQuery = testExamQuery.Where(te => te.Subject.SubjectName.Contains(subjectName));
        }

        if (!string.IsNullOrEmpty(department))
        {
            testExamQuery = testExamQuery.Where(te => te.Department.Name.Contains(department));
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
        // Create a new Assignment first
        var assignment = new Assignment
        {
            UserId = UserId,
            TestExamId = request.TestExamId,
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

        // Iterate over the answer IDs provided in the request
        foreach (var answerId in request.AnswerIds)
        {
            var answer = await _context.Answers
                .Include(a => a.Question)
                .FirstOrDefaultAsync(a => a.Id == answerId);

            if (answer != null && answer.Question != null)
            {
                bool isCorrect = answer.IsCorrect ?? false;

                // If the answer is correct, add the question's mark to the total score
                if (isCorrect)
                {
                    totalScore += answer.Question.Mark ?? 0;
                }

                // Create and add AssignmentDetail for each answer
                _context.AssignmentDetails.Add(new AssignmentDetail
                {
                    AssignmentId = assignment.Id, // Use the ID of the newly created assignment
                    AnswerId = answerId,
                    IsCorrect = isCorrect,
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now
                });
            }
        }

        // Update the total score for the Assignment
        assignment.TotalScore = totalScore;

        // Save changes (this will also save the AssignmentDetails)
        await _context.SaveChangesAsync();

        return new ApiResponse<object>(0, "Nộp bài thành công!", new { TotalScore = totalScore });
    }


    public async Task<ApiResponse<object>> SaveEssay(int UserId, SaveEssayRequest request)
    {
        // Kiểm tra xem đã có bài nộp chưa
        var existingAssignment = await _context.Assignments
            .FirstOrDefaultAsync(a => a.UserId == UserId && a.TestExamId == request.TestExamId);

        string fileUrl = await SaveEssayToFile(request.SubmissionFile);

        if (existingAssignment != null)
        {
            // Nếu bài đã tồn tại -> Cập nhật nội dung mới
            existingAssignment.SubmissionFile = fileUrl;
            _context.Assignments.Update(existingAssignment);
        }
        else
        {
            // Nếu chưa có bài -> Tạo mới
            var assignment = new Assignment
            {
                UserId = UserId,
                TestExamId = request.TestExamId,
                SubmissionFile = fileUrl,
                SubmissionDate = DateTime.Now,
                IsSubmit = true
            };
            await _context.Assignments.AddAsync(assignment);
            await _context.SaveChangesAsync();
            var fileFormats = await _context.FileFormats
                .Where(ts => ts.TestExamId == request.TestExamId)
                .ToListAsync();

            var cloudinaryService = _serviceProvider.GetService<ICloudinaryService>();
            long fileSize = 0; // Initialize fileSize outside the loop
            string base64Data = string.Empty;
            byte[] fileBytes = null;
            string uploadResult = null;

            if (!string.IsNullOrEmpty(request.AttachedFile))
            {
                // Xác định dung lượng tối đa cho file
                long maxSize = 0;

                foreach (var file in fileFormats)
                {
                    // Kiểm tra loại file và dung lượng tối đa
                    if (file.IsDoc == true)
                    {
                        if (file.Is10 == true) maxSize = Math.Max(maxSize, 10 * 1024 * 1024);
                        if (file.Is20 == true) maxSize = Math.Max(maxSize, 20 * 1024 * 1024);
                        if (file.Is30 == true) maxSize = Math.Max(maxSize, 30 * 1024 * 1024);
                        if (file.Is40 == true) maxSize = Math.Max(maxSize, 40 * 1024 * 1024);

                        var match = Regex.Match(request.AttachedFile, @"data:application/msword;base64,(?<data>.*)");
                        if (match.Success)
                        {
                            base64Data = match.Groups["data"].Value;
                            fileBytes = Convert.FromBase64String(base64Data);
                            fileSize = fileBytes.Length; // Calculate file size from base64 data
                        }
                        uploadResult = await cloudinaryService.UploadDocxAsync(request.AttachedFile);
                    }

                    // Kiểm tra các loại file khác như PowerPoint, Excel, JPEG
                    if (file.IsPowerpoint == true &&
                        Regex.IsMatch(request.AttachedFile, @"data:application/vnd.ms-powerpoint;base64,"))
                    {
                        var match = Regex.Match(request.AttachedFile,
                            @"data:application/vnd.ms-powerpoint;base64,(?<data>.*)");
                        if (match.Success)
                        {
                            base64Data = match.Groups["data"].Value;
                            fileBytes = Convert.FromBase64String(base64Data);
                            fileSize = fileBytes.Length;
                        }
                        uploadResult = await cloudinaryService.UploadPowerPointAsync(request.AttachedFile);
                    }

                    if (file.IsXxls == true &&
                        Regex.IsMatch(request.AttachedFile, @"data:application/vnd.ms-excel;base64,"))
                    {
                        var match = Regex.Match(request.AttachedFile,
                            @"data:application/vnd.ms-excel;base64,(?<data>.*)");
                        if (match.Success)
                        {
                            base64Data = match.Groups["data"].Value;
                            fileBytes = Convert.FromBase64String(base64Data);
                            fileSize = fileBytes.Length;
                        }
                        uploadResult = await cloudinaryService.UploadExcelAsync(request.AttachedFile);
                    }

                    if (file.IsJpeg == true && Regex.IsMatch(request.AttachedFile, @"data:image/jpeg;base64,"))
                    {
                        var match = Regex.Match(request.AttachedFile, @"data:image/jpeg;base64,(?<data>.*)");
                        if (match.Success)
                        {
                            base64Data = match.Groups["data"].Value;
                            fileBytes = Convert.FromBase64String(base64Data);
                            fileSize = fileBytes.Length;
                        }
                        uploadResult = await cloudinaryService.UploadImageAsync(request.AttachedFile);
                    }
                }

                // Kiểm tra dung lượng file
                if (fileSize <= maxSize)
                {
                    // Nếu upload thành công
                    if (!string.IsNullOrEmpty(uploadResult))
                    {
                        var existingSubmissionFile = await _context.SubmissionFiles
                            .FirstOrDefaultAsync(sf => sf.AssignmentId == assignment.Id);

                        if (existingSubmissionFile != null)
                        {
                       
                            existingSubmissionFile.FileName = uploadResult;
                            _context.SubmissionFiles.Update(existingSubmissionFile);
                        }
                        else
                        {
                    
                            var submissionFile = new SubmissionFile
                            {
                                AssignmentId = assignment.Id,
                                FileName = uploadResult
                            };
                            await _context.SubmissionFiles.AddAsync(submissionFile);
                        }
                    }
                    else
                    {
                        return new ApiResponse<object>(1, "Lỗi khi tải file lên Cloudinary!", null);
                    }
                }
                else
                {
                    // Nếu file vượt quá dung lượng cho phép
                    return new ApiResponse<object>(1,
                        $"File vượt quá dung lượng cho phép! Tối đa {maxSize / (1024 * 1024)}MB.",
                        null);
                }
            }
        }

        await _context.SaveChangesAsync();
        return new ApiResponse<object>(0, "Nộp bài thành công!", null);
    }


    public async Task<string> SaveEssayToFile(string content)
    {
        // Tạo đường dẫn file tạm thời trong thư mục hệ thống
        string filePath = Path.Combine(Path.GetTempPath(), "abc.docx");

        // Tạo file DOCX
        using (WordprocessingDocument wordDocument =
               WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
        {
            MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
            mainPart.Document = new Document();
            Body body = new Body();
            Paragraph paragraph = new Paragraph();
            Run run = new Run();
            Text textElement = new Text(content); // Ghi nội dung

            run.Append(textElement);
            paragraph.Append(run);
            body.Append(paragraph);
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