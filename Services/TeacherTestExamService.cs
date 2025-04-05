using AutoMapper;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http;
using Project_LMS.Config;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Helpers;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services;

public class TeacherTestExamService : ITeacherTestExamService
{
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;
    private readonly IHttpClientFactory _httpClientFactory;

    public TeacherTestExamService(ApplicationDbContext context, IServiceProvider serviceProvider, IMapper mapper,
        IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _serviceProvider = serviceProvider;
        _mapper = mapper;
        _httpClientFactory = httpClientFactory;
    }


    public async Task<ApiResponse<PaginatedResponse<TeacherTestExamResponse>>> GetTeacherTestExamAsync(
       int userId, int? pageNumber, int? pageSize, string? sortDirection, string? topicName,
        int? subjectId, int? departmentId, string? startDate , string? tab)
    {
        
        var today = DateTime.Now.Date;

        var testExams = await _context.TestExams
            .Where(te => te.IsDelete  ==false
                         && te.StartDate.HasValue 
                         && te.EndDate.HasValue
                         && te.UserId == userId
            ).ToListAsync();
        foreach (var testExam in testExams)
        {
            // Nếu StartDate là hôm nay, cập nhật trạng thái thành 6
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

        await _context.SaveChangesAsync();
        
        if (pageNumber.HasValue && pageNumber <= 0)
        {
            return new ApiResponse<PaginatedResponse<TeacherTestExamResponse>>(
                1, "Giá trị pageNumber phải lớn hơn 0", null);
        }

        if (pageSize.HasValue && pageSize <= 0)
        {
            return new ApiResponse<PaginatedResponse<TeacherTestExamResponse>>(
                1, "Giá trị pageSize phải lớn hơn 0", null);
        }

        if (!string.IsNullOrEmpty(sortDirection) &&
            !sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase) &&
            !sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
        {
            return new ApiResponse<PaginatedResponse<TeacherTestExamResponse>>(
                1, "Giá trị sortDirection phải là 'asc' hoặc 'desc'", null);
        }

        try
        {
            var currentPage = pageNumber ?? 1;
            var currentPageSize = pageSize ?? 10;


            var testExamQuery = _context.TestExams
                .Where(ts => ts.IsExam == false && ts.IsDelete == false)
                .Include(te => te.ClassTestExams)
                .ThenInclude(ct => ct.Class)
                .Include(te => te.Subject)
                .Include(te => te.ExamScheduleStatus)
                .AsQueryable();

            // **Apply filtering**
            if (!string.IsNullOrEmpty(topicName))
            {
                var decodedTopicName = Uri.UnescapeDataString(topicName); // Decodes "Ki%E1%BB%83m%20" to "Kiểm "
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

          
            
            if (!string.IsNullOrEmpty(tab))
            {
                if (tab.Equals("upcoming", StringComparison.OrdinalIgnoreCase))
                {
                    testExamQuery = testExamQuery.Where(te => te.ScheduleStatusId == 3);
                }
            }
            
                

            if (!string.IsNullOrEmpty(startDate) && DateTime.TryParseExact(startDate, "dd-MM-yyyy",
                    new System.Globalization.CultureInfo("vi-VN"), System.Globalization.DateTimeStyles.None,
                    out var parsedDate))
            {
                testExamQuery = testExamQuery.Where(te =>
                    te.StartDate.HasValue && te.StartDate.Value.Date == parsedDate.Date);
            }
            
            // **Apply sorting**
            sortDirection ??= "asc";
            testExamQuery = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
                ? testExamQuery.OrderByDescending(te => te.StartDate)
                : testExamQuery.OrderBy(te => te.StartDate);

            // **Calculate pagination**
            var totalItems = await testExamQuery.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / currentPageSize);

            var pagedTestExams = await testExamQuery
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToListAsync();
           
            var mappedData = new List<TeacherTestExamResponse>();

            foreach (var testExam in pagedTestExams)
            {
                foreach (var classTestExam in testExam.ClassTestExams)
                {
                    // Lấy tất cả học sinh trong lớp
                    var classStudents = await _context.ClassStudents
                        .Where(cs => cs.ClassId == classTestExam.ClassId)
                        .ToListAsync();

// Mặc định giả sử tất cả học sinh đã được chấm
                    bool allGraded = true;

                    foreach (var classStudent in classStudents)
                    {
                        var assignment = await _context.Assignments
                            .Where(a => a.UserId == classStudent.Id && a.TestExamId == testExam.Id)
                            .FirstOrDefaultAsync();

                        // Nếu học sinh không có bài nộp (không có assignment), coi là đã chấm
                        if (assignment == null || assignment.TotalScore == null)
                        {
                            allGraded = false;
                            break; 
                        }
                    }

                    if (!string.IsNullOrEmpty(tab))
                    {
                        if (tab.Equals("no_graded", StringComparison.OrdinalIgnoreCase))
                        {
                            allGraded = false;  // Nếu tab là "no_graded", đánh dấu là chưa chấm
                        }
                        else if (tab.Equals("graded", StringComparison.OrdinalIgnoreCase))
                        {
                            // Nếu tab là "graded", đánh dấu là đã chấm
                            allGraded = true;
                        }
                    }
                    
                    string status = allGraded ? "Đã chấm" : "Chưa chấm";



                    // Cập nhật thông tin cho DTO
                    mappedData.Add(new TeacherTestExamResponse
                    {
                        Id = testExam.Id,
                        classId = classTestExam.ClassId,
                        ClassName = classTestExam.Class.Name,
                        ContentTest = testExam.Topic,
                        SubjectName = testExam.Subject.SubjectName,
                        DateOfExam = testExam.StartDate.HasValue ? testExam.StartDate.Value.ToString("dddd, dd-MM-yyyy, HH:mm") : string.Empty,
                        Duration = testExam.Duration.HasValue ? testExam.Duration.Value.ToString(@"hh\:mm") : string.Empty,
                        Status = testExam.ExamScheduleStatus.Names,
                        Graded = status // Hiển thị trạng thái chấm điểm cho lớp này
                    });
                }
            }
            
            // **Create paginated response**
            var paginatedResponse = new PaginatedResponse<TeacherTestExamResponse>
            {
                Items = mappedData,
                PageNumber = currentPage,
                PageSize = currentPageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = currentPage > 1,
                HasNextPage = currentPage < totalPages
            };

            return new ApiResponse<PaginatedResponse<TeacherTestExamResponse>>(
                0, "Lấy dữ liệu thành công", paginatedResponse);
        }
        catch (Exception ex)
        {
            return new ApiResponse<PaginatedResponse<TeacherTestExamResponse>>(
                1, $"Lỗi: {ex.Message}", null);
        }
    }

    

    public async Task<ApiResponse<object?>> CreateTeacherTestExamAsync(int userId, TeacherTestExamRequest request)
    {
        // Kiểm tra xem giáo viên có được phân công dạy môn học này không
        bool isAssigned = await _context.TeachingAssignments
            .AnyAsync(ta => ta.UserId == userId && ta.SubjectId == request.SubjectId);

        if (!isAssigned)
        {
            return new ApiResponse<object?>(1, "Giáo viên không được phân công dạy môn học này.", null);
        }

        
        
        // Tìm danh sách lớp dựa vào DepartmentId hoặc classIds
        List<int> classIds = new List<int>();

        if (request.SelectALL)
        {
            classIds = await _context.Classes
                .Where(c => c.DepartmentId == request.DepartmentId)
                .Select(c => c.Id)
                .ToListAsync();
        }
        else if (request.classIds != null && request.classIds.Any())
        {
            classIds = request.classIds;
        }


        classIds = await _context.ClassSubjects
            .Where(cs => cs.SubjectId == request.SubjectId && classIds.Contains(cs.ClassId.Value))
            .Select(cs => cs.ClassId.Value)
            .Distinct()
            .ToListAsync();


        if (!classIds.Any())
        {
            return new ApiResponse<object?>(1, "Không có lớp nào trong danh sách được dạy môn học này.", null);
        }


        // Convert Duration string thành kiểu TimeOnly nếu có
        TimeOnly? duration = TimeOnly.TryParse(request.Duration, out var parsedDuration) ? parsedDuration : null;

        var existingTestExam = await _context.TestExams
            .FirstOrDefaultAsync(te => te.StartDate == request.StartDate);


        if (existingTestExam != null)
        {
            return new ApiResponse<object?>(1, "Không được trùng lịch kiểm tra ", null);
        }

        if (request.EndDate <= request.StartDate)
        {
            return new ApiResponse<object?>(1, "Ngày kết thúc phải lớn hơn ngày bắt đầu.", null);
        }


        // Tạo đối tượng TestExam
        var teacherTestExam = new TestExam
        {
            SubjectId = request.SubjectId,
            DepartmentId = request.DepartmentId,
            Topic = request.Topic,
            Form = request.Form,
            Duration = duration,
            TestExamTypeId = request.TestExamType,
            StartDate = request.StartDate.ToOffset(TimeSpan.FromHours(7)),
            EndDate = request.EndDate.ToOffset(TimeSpan.FromHours(7)),
            Description = request.Description,
            IsAttachmentRequired = request.IsAttachmentRequired,
            ScheduleStatusId = 2,
            UserId = userId,
        };


        // Xử lý file đính kèm nếu có
        if (!string.IsNullOrEmpty(request.Attachment))
        {
            try
            {
                var cloudinaryService = _serviceProvider.GetService<ICloudinaryService>();
                if (cloudinaryService == null)
                {
                    throw new Exception("Cloudinary service not available");
                }

                teacherTestExam.Attachment = await cloudinaryService.UploadDocxAsync(request.Attachment);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi upload file
                return new ApiResponse<object?>(1, "File upload failed: " + ex.Message);
            }
        }

        // Lưu TestExam vào cơ sở dữ liệu
        await _context.TestExams.AddAsync(teacherTestExam);
        await _context.SaveChangesAsync(); // Lưu dữ liệu TestExam


        var afteraddtestExam = await _context.TestExams.FindAsync(teacherTestExam.Id);
        if (request.IsAttachmentRequired)
        {
            var fileFormat = new FileFormat
            {
                TestExamId = teacherTestExam.Id,
                IsDoc = request.IsDoc,
                IsPowerpoint = request.IsPowerpoint,
                IsXxls = request.IsXxls,
                IsJpeg = request.IsJpeg,
                Is10 = request.Is10,
                Is20 = request.Is20,
                Is30 = request.Is30,
                Is40 = request.Is40,
            };

            await _context.FileFormats.AddAsync(fileFormat);
            await _context.SaveChangesAsync();
        }

        var fileProcessor = new FileProcessor(_httpClientFactory);
        string fileUrl = afteraddtestExam.Attachment; // Lấy URL file từ TestExam

        // Đọc danh sách câu hỏi từ file
        List<Question> questions = await fileProcessor.ReadQuestionsFromFileAsync(fileUrl);
        var newAnswers = new List<Answer>();

        // Lưu câu hỏi và đáp án vào cơ sở dữ liệu

        foreach (var question in questions)
        {
            var newQuestion = new Question
            {
                QuestionText = question.QuestionText,
                QuestionType = question.QuestionType,
                Mark = question.Mark,
                TestExamId = teacherTestExam.Id // Liên kết với TestExam
            };

            await _context.Questions.AddAsync(newQuestion);
            await _context.SaveChangesAsync(); // Lưu câu hỏi

            foreach (var answer in question.Answers)
            {
                var newAnswer = new Answer
                {
                    Answer1 = answer.Answer1,
                    IsCorrect = answer.IsCorrect,
                    QuestionId = newQuestion.Id, // Liên kết với câu hỏi
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now
                };

                await _context.Answers.AddAsync(newAnswer);
            }

            await _context.SaveChangesAsync();
        }

        // Thêm quan hệ vào bảng trung gian ClassTestExam
        var testExamClasses = classIds.Select(classId => new ClassTestExam
        {
            TestExamId = teacherTestExam.Id,
            ClassId = classId
        }).ToList();

        await _context.ClassTestExams.AddRangeAsync(testExamClasses);
        await _context.SaveChangesAsync();

        // Tạo phản hồi với thông tin đã lưu
        var responseData = new
        {
            SubjectName = _context.Subjects
                .Where(s => s.Id == teacherTestExam.SubjectId)
                .Select(s => s.SubjectName)
                .FirstOrDefault(),
            TestExamId = teacherTestExam.Id,
            DepartmentId = teacherTestExam.DepartmentId,
            Topic = teacherTestExam.Topic,
            Form = teacherTestExam.Form,
            Duration = teacherTestExam.Duration,
            TestExamTypeId = teacherTestExam.TestExamTypeId,
            StartDate = teacherTestExam.StartDate,
            EndDate = teacherTestExam.EndDate,
            Description = teacherTestExam.Description,
            IsAttachmentRequired = teacherTestExam.IsAttachmentRequired,
            ClassIds = testExamClasses.Select(tc => tc.ClassId).ToList()
        };

        return new ApiResponse<object?>(0, "Tạo lịch kiểm tra thành công", responseData);
    }


    public async Task<ApiResponse<object?>> UpdateTeacherTestExamAsync(int userId, TeacherTestExamRequest request)
    {
        var testExamId = request.Id;
        var teacherTestExam = await _context.TestExams.FindAsync(testExamId);
        if (teacherTestExam == null)
        {
            return new ApiResponse<object?>(-1, "Không tìm thấy lịch thi", null);
        }

        var question = await _context.Questions.Where(qs => qs.TestExamId == testExamId).ToListAsync();
        foreach (var anwer in question)
        {
            var answer = await _context.Answers.Where(a => a.QuestionId == anwer.Id).ToListAsync();
            _context.Answers.RemoveRange(answer);
        }

        _context.Questions.RemoveRange(question);
        await _context.SaveChangesAsync();

        // Tìm danh sách lớp cần cập nhật
        List<int> classIds = new List<int>();
        if (request.SelectALL)
        {
            classIds = await _context.Classes
                .Where(c => c.DepartmentId == request.DepartmentId)
                .Select(c => c.Id)
                .ToListAsync();
        }
        else if (request.classIds != null && request.classIds.Any())
        {
            classIds = request.classIds;
        }

        if (request.IsAttachmentRequired)
        {
            var existingFileFormat = await _context.FileFormats
                .FirstOrDefaultAsync(ff => ff.TestExamId == request.Id);

            if (existingFileFormat != null)
            {
                existingFileFormat.IsDoc = request.IsDoc;
                existingFileFormat.IsPowerpoint = request.IsPowerpoint;
                existingFileFormat.IsXxls = request.IsXxls;
                existingFileFormat.IsJpeg = request.IsJpeg;
                existingFileFormat.Is10 = request.Is10;
                existingFileFormat.Is20 = request.Is20;
                existingFileFormat.Is30 = request.Is30;
                existingFileFormat.Is40 = request.Is40;
                _context.FileFormats.Update(existingFileFormat);
            }
            else
            {
                var fileFormat = new FileFormat
                {
                    TestExamId = request.Id,
                    IsDoc = request.IsDoc,
                    IsPowerpoint = request.IsPowerpoint,
                    IsXxls = request.IsXxls,
                    IsJpeg = request.IsJpeg,
                    Is10 = request.Is10,
                    Is20 = request.Is20,
                    Is30 = request.Is30,
                    Is40 = request.Is40,
                };
                await _context.FileFormats.AddAsync(fileFormat);
            }

            await _context.SaveChangesAsync();
        }

        // Chuyển đổi thời gian
        TimeOnly? duration = TimeOnly.TryParse(request.Duration, out var parsedDuration) ? parsedDuration : null;

        // Cập nhật thông tin của TestExam
        teacherTestExam.SubjectId = request.SubjectId;
        teacherTestExam.DepartmentId = request.DepartmentId;
        teacherTestExam.Topic = request.Topic;
        teacherTestExam.Form = request.Form;
        teacherTestExam.Duration = duration;
        teacherTestExam.TestExamTypeId = request.TestExamType;
        teacherTestExam.StartDate = request.StartDate.ToOffset(TimeSpan.FromHours(7));
        teacherTestExam.EndDate = request.EndDate.ToOffset(TimeSpan.FromHours(7));
        teacherTestExam.Description = request.Description;
        teacherTestExam.UserId = userId;
        teacherTestExam.IsAttachmentRequired = request.IsAttachmentRequired;


        if (!string.IsNullOrEmpty(request.Attachment))
        {
            // Assuming the attachment is a base64 string
            try
            {
                var cloudinaryService = _serviceProvider.GetService<ICloudinaryService>();
                if (cloudinaryService == null)
                {
                    throw new Exception("Cloudinary service not available");
                }

                teacherTestExam.Attachment = await cloudinaryService.UploadDocAsync(request.Attachment);
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during file upload
                return new ApiResponse<object?>(1, "File upload failed: " + ex.Message);
            }
        }

        _context.TestExams.Update(teacherTestExam);
        await _context.SaveChangesAsync();

        // Xóa các lớp cũ liên kết với TestExam
        var existingClasses = _context.ClassTestExams.Where(cte => cte.TestExamId == testExamId);
        _context.ClassTestExams.RemoveRange(existingClasses);
        await _context.SaveChangesAsync();

        // Thêm quan hệ mới vào bảng trung gian
        var testExamClasses = classIds.Select(classId => new ClassTestExam
        {
            TestExamId = teacherTestExam.Id,
            ClassId = classId
        }).ToList();

        await _context.ClassTestExams.AddRangeAsync(testExamClasses);
        await _context.SaveChangesAsync();

        // Trả về dữ liệu sau khi cập nhật
        var responseData = new
        {
            SubjectName = await _context.Subjects
                .Where(s => s.Id == teacherTestExam.SubjectId)
                .Select(s => s.SubjectName)
                .FirstOrDefaultAsync(),
            TestExamId = teacherTestExam.Id,
            DepartmentId = teacherTestExam.DepartmentId,
            Topic = teacherTestExam.Topic,
            Form = teacherTestExam.Form,
            Duration = teacherTestExam.Duration,
            TestExamTypeId = teacherTestExam.TestExamTypeId,
            StartDate = teacherTestExam.StartDate,
            EndDate = teacherTestExam.EndDate,
            Description = teacherTestExam.Description,
            IsAttachmentRequired = teacherTestExam.IsAttachmentRequired,
            ClassIds = testExamClasses.Select(tc => tc.ClassId).ToList()
        };

        return new ApiResponse<object?>(0, "Cập nhật kiểm tra thành công", responseData);
    }

    public async Task<ApiResponse<object?>> GetTeacherTestExamById(int id, int classId)
    {
        // Tìm kiếm TestExam theo ID và ClassId
        var testExam = await _context.TestExams
            .Where(ts => ts.IsExam == false && ts.IsDelete == false)
            .Include(te => te.Subject) // Lấy thông tin Subject
            .Include(te => te.ClassTestExams) // Lấy thông tin ClassTestExams (bảng trung gian)
            .ThenInclude(ct => ct.Class) // Lấy thông tin ClassName từ bảng trung gian
            .FirstOrDefaultAsync(te =>
                te.Id == id && te.ClassTestExams.Any(ct => ct.ClassId == classId)); // Kiểm tra ClassId

        if (testExam == null)
        {
            return new ApiResponse<object?>(1, "Không tìm thấy lịch kiểm tra cho lớp học này", null);
        }

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

        int quantity = await _context.TestExams
            .Where(te => te.SubjectId == testExam.SubjectId && te.IsExam == false && te.IsDelete == false &&
                         te.ClassTestExams.Any(ct => ct.ClassId == classId)) // Filter by the classId
            .CountAsync();


        // Lấy danh sách bài kiểm tra liên quan, không dùng list nữa
        var relatedTestsRaw = await _context.TestExams
            .Where(te => te.SubjectId == testExam.SubjectId &&
                         te.IsExam == false &&
                         te.IsDelete == false &&
                         te.ClassTestExams.Any(ct => ct.ClassId == classId)) // Filter by classId directly
            .OrderBy(te => te.StartDate)
            .ThenBy(te => te.Id)
            .ToListAsync();

        // Gán số thứ tự câu hỏi theo thứ tự xuất hiện
        var relatedTests = relatedTestsRaw
            .Select((te, index) => new RelatedTestDTO
            {
                ExamNumber = $"CÂU {index + 1}", // Đánh số từ 1
                StartDate = te.StartDate.HasValue ? te.StartDate.Value.ToString("dd-MM-yyyy HH:mm") : "Chưa xác định"
            })
            .ToList();

        var responseData = new TeacherTestExamDetailResponse
        {
            StartDate = testExam.StartDate.HasValue ? testExam.StartDate.Value.ToString("dd-MM-yyyy") : "Chưa xác định",
            SubjectName = testExam.Subject.SubjectName,
            ClassName = string.Join(", ",
                testExam.ClassTestExams.Where(te => te.ClassId == classId).Select(e => e.Class.Name)),
            Duration = durationString,
            Description = testExam.Description,
            Attachment = testExam.Attachment,
            IsAttachmentRequired = testExam.IsAttachmentRequired,
            Quantity = quantity,
            RelatedTests = relatedTests
        };

        return new ApiResponse<object?>(0, "Fill dữ liệu lịch kiểm tra thành công", responseData);
    }


    public async Task<ApiResponse<object?>> GetFilterClass(int departmentId)
    {
        try
        {
            var classes = await _context.Classes
                .Where(c => c.DepartmentId == departmentId)
                .Select(c => new { c.Id, c.Name })
                .ToListAsync();
            return new ApiResponse<object?>(0, "Fill class thành công", classes);
        }
        catch (Exception ex)
        {
            return new ApiResponse<object?>(0, $"Lỗi: {ex.Message}", null);
        }
    }

    public async Task<ApiResponse<object?>> DeleteTeacherTestExamById(int id)
    {
        try
        {
            var testExam = await _context.TestExams
                .Where(ts => ts.IsExam == false && ts.Id == id)
                .FirstOrDefaultAsync();
            if (testExam == null || testExam.IsDelete == true)
                return new ApiResponse<object>(1, "TestExam not found or already deleted", false);

            // Set the TestExam IsDelete to true
            testExam.IsDelete = true;

            // Find and update the related ClassTestExam records
            var classTestExams = await _context.ClassTestExams
                .Where(cte => cte.TestExamId == id)
                .ToListAsync();

            foreach (var classTestExam in classTestExams)
            {
                classTestExam.IsDelete = true;
            }

            // Save the changes to both TestExam and ClassTestExam
            await _context.SaveChangesAsync();

            return new ApiResponse<object>(0, "TestExam and related ClassTestExam records deleted successfully", true);
        }
        catch (Exception ex)
        {
            return new ApiResponse<object>(1,
                $"Error deleting TestExam and related ClassTestExam records: {ex.Message}", false);
        }
    }
}