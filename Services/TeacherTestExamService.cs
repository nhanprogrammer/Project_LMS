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
        int? pageNumber, int? pageSize, string? sortDirection, string? topicName,
        string? subjectName, string? department, string? startDate)
    {
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
                .AsQueryable();

            // **Apply filtering**
            if (!string.IsNullOrEmpty(topicName))
            {
                var decodedTopicName = Uri.UnescapeDataString(topicName); // Decodes "Ki%E1%BB%83m%20" to "Kiểm "
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

            // **Map data to DTO**
            var mappedData = _mapper.Map<List<TeacherTestExamResponse>>(pagedTestExams);

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


    public async Task<ApiResponse<object?>> CreateTeacherTestExamAsync(TeacherTestExamRequest request)
    {
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


       
        // Convert Duration string thành kiểu TimeOnly nếu có
        TimeOnly? duration = TimeOnly.TryParse(request.Duration, out var parsedDuration) ? parsedDuration : null;

        var existingTestExam = await _context.TestExams
            .FirstOrDefaultAsync(te => te.StartDate == request.StartDate);
        
        
        if (existingTestExam != null)
        {
            return new ApiResponse<object?>(1, "Không được trùng " , null);
        }

        if (request.EndDate == request.StartDate)
        {
            return new ApiResponse<object?>(1, "Ngày bất đầu và ngày kết thúc không được trùng" , null);
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
            ScheduleStatusId = 2
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


    public async Task<ApiResponse<object?>> UpdateTeacherTestExamAsync(TeacherTestExamRequest request)
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

    public async Task<ApiResponse<object?>> GetTeacherTestExamById(int id)
    {
        // Tìm kiếm đối tượng TestExam theo ID
        var testExam = await _context.TestExams
            .Where(ts => ts.IsExam == false && ts.IsDelete == false)
            .Include(te => te.Subject) // Lấy thông tin Subject
            .Include(te => te.ClassTestExams) // Lấy thông tin ClassTestExams (bảng trung gian)
            .ThenInclude(ct => ct.Class) // Lấy thông tin ClassName từ bảng trung gian
            .FirstOrDefaultAsync(te => te.Id == id); // Tìm kiếm theo ID

        if (testExam == null)
        {
            return new ApiResponse<object?>(1, "Không tìm thấy lịch kiểm tra", null);
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
                         te.ClassTestExams.Any(
                             ct => testExam.ClassTestExams.Select(c => c.ClassId).Contains(ct.ClassId)))
            .CountAsync();

        var testExamClassIds = testExam.ClassTestExams.Select(c => c.ClassId).ToList();

        var relatedTests = await _context.TestExams
            .Where(te => te.SubjectId == testExam.SubjectId &&
                         te.IsExam == false &&
                         te.IsDelete == false &&
                         te.ClassTestExams.Any(ct => testExamClassIds.Contains(ct.ClassId)))
            .OrderBy(te => te.StartDate)
            .ThenBy(te => te.Id)
            .Select(te => new RelatedTestDTO
            {
                ExamNumber = te.Id,
                StartDate = te.StartDate.HasValue ? te.StartDate.Value.ToString("dd-MM-yyyy HH:mm") : "Chưa xác định"
            })
            .ToListAsync();

        var responseData = new TeacherTestExamDetailResponse
        {
            StartDate = testExam.StartDate.HasValue ? testExam.StartDate.Value.ToString("dd-MM-yyyy") : "Chưa xác định",
            SubjectName = testExam.Subject.SubjectName,
            ClassList = string.Join(", ", testExam.ClassTestExams.Select(e => e.Class.Name)),
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