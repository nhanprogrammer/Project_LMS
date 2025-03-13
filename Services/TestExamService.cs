using System.Net.Mail;
using System.Net.WebSockets;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;


namespace Project_LMS.Services;

public class TestExamService : ITestExamService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public TestExamService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PaginatedResponse<TestExamResponse>>> GetAllTestExamsAsync(string? keyword,
        int pageNumber, int pageSize)
    {
        // var query = _context.TestExams
        //     .Where(te => !te.IsDelete.HasValue || !te.IsDelete.Value);
        //
        // if (!string.IsNullOrWhiteSpace(keyword))
        // {
        //     keyword = keyword.Trim().ToLower();
        //     query = query.Where(te =>
        //         (te.Topic != null && te.Topic.ToLower().Contains(keyword)) ||
        //         (te.Description != null && te.Description.ToLower().Contains(keyword))
        //     );
        // }
        //
        // query = query.Include(te => te.Department).Include(te => te.TestExamType);
        //
        // var totalItems = await query.CountAsync();
        // var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        //
        // var testExams = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        //
        // var testExamResponses = testExams.Select(te => new TestExamResponse
        // {
        //     Id = te.Id,
        //     DepartmentId = te.DepartmentId,
        //     TestExamTypeId = te.TestExamTypeId,
        //     Topic = te.Topic,
        //     Form = te.Form,
        //     Duration = te.Duration,
        //     StartDate = te.StartDate,
        //     EndDate = te.EndDate,
        //     Description = te.Description,
        //     Attachment = te.Attachment
        // }).ToList();
        //
        // var paginatedResponse = new PaginatedResponse<TestExamResponse>
        // {
        //     Items = testExamResponses,
        //     PageNumber = pageNumber,
        //     PageSize = pageSize,
        //     TotalItems = totalItems,
        //     TotalPages = totalPages,
        //     HasPreviousPage = pageNumber > 1,
        //     HasNextPage = pageNumber < totalPages
        // };
        //
        // return new ApiResponse<PaginatedResponse<TestExamResponse>>(0, "Success", paginatedResponse);
        throw new Exception();
    }

    public async Task<ApiResponse<TestExamResponse>> GetTestExamByIdAsync(int id)
    {
        // var testExam = await _context.TestExams
        //     .Include(te => te.Department)
        //     .Include(te => te.TestExamType)
        //     .FirstOrDefaultAsync(te => te.Id == id && (!te.IsDelete.HasValue || !te.IsDelete.Value));
        //
        // if (testExam == null)
        //     return new ApiResponse<TestExamResponse>(1, "TestExam not found", null);
        //
        // var response = new TestExamResponse
        // {
        //     Id = testExam.Id,
        //     DepartmentId = testExam.DepartmentId,
        //     TestExamTypeId = testExam.TestExamTypeId,
        //     Topic = testExam.Topic,
        //     Form = testExam.Form,
        //     Duration = testExam.Duration,
        //     StartDate = testExam.StartDate,
        //     EndDate = testExam.EndDate,
        //     Description = testExam.Description,
        //     Attachment = testExam.Attachment
        // };
        // return new ApiResponse<TestExamResponse>(0, "Success", response);
        throw new Exception();
    }

    public async Task<ApiResponse<object>> CreateTestExamAsync(CreateTestExamRequest request)
    {
        // 1) Kiểm tra Học kỳ (SemestersId) có tồn tại không
        if (request.SemestersId.HasValue)
        {
            bool existsSemester = await _context.Semesters
                .AnyAsync(s => s.Id == request.SemestersId.Value);
            if (!existsSemester)
            {
                return new ApiResponse<object>(1, "Học kỳ không tồn tại trong DB", null);
            }
        }

        // 2) Kiểm tra Môn học (SubjectId) có tồn tại không
        if (request.SubjectId.HasValue)
        {
            bool existsSubject = await _context.Subjects
                .AnyAsync(sub => sub.Id == request.SubjectId.Value);
            if (!existsSubject)
            {
                return new ApiResponse<object>(1, "Môn học không tồn tại trong DB", null);
            }
        }

        // 3) Kiểm tra Loại kiểm tra (TestExamTypeId) có tồn tại không
        if (request.TestExamTypeId.HasValue)
        {
            bool existsType = await _context.TestExamTypes
                .AnyAsync(t => t.Id == request.TestExamTypeId.Value);
            if (!existsType)
            {
                return new ApiResponse<object>(1, "Loại kiểm tra không tồn tại trong DB", null);
            }
        }

        // 4) Map Request -> TestExam
        var testExam = _mapper.Map<TestExam>(request);
        testExam.ScheduleStatusId = 1;
        testExam.DepartmentId = request.DepartmentId;
        // 5) Add TestExam vào DbSet
        await _context.TestExams.AddAsync(testExam);

        // 6) SaveChanges để có testExam.Id
        await _context.SaveChangesAsync();

        // 7) Xử lý ClassOption & ClassIds => thêm ClassTestExams
        var finalClassIds = new List<int>();

        if (request.ClassOption == "ALL")
        {
            finalClassIds = await _context.Classes
                .Where(c => c.IsDelete == false)
                .Select(c => c.Id)
                .ToListAsync();
        }
        else if (request.ClassOption == "TYPE")
        {
            if (request.SelectedClassTypeId.HasValue)
            {
                var typeId = request.SelectedClassTypeId.Value;
                finalClassIds = await _context.Classes
                    .Where(c => c.ClassTypeId == typeId && c.IsDelete == false)
                    .Select(c => c.Id)
                    .ToListAsync();
            }
        }
        else
        {
            if (request.ClassIds != null && request.ClassIds.Any())
            {
                finalClassIds = request.ClassIds;
            }
        }

        // 8) Tạo bản ghi ClassTestExam
        foreach (var classId in finalClassIds.Distinct())
        {
            var classTestExam = new ClassTestExam
            {
                TestExamId = testExam.Id,
                ClassId = classId,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                IsDelete = false
            };
            _context.ClassTestExams.Add(classTestExam);
        }

        // 9) Xử lý phân công chấm thi (Examiner)
        //    Nếu ApplyExaminerForAllClasses = true => examiner chung áp dụng cho tất cả các lớp
        if (request.ApplyExaminerForAllClasses == true)
        {
            if (request.ExaminerIds != null && request.ExaminerIds.Any())
            {
                foreach (var examinerId in request.ExaminerIds)
                {
                    var examiner = new Examiner
                    {
                        TestExamId = testExam.Id,
                        UserId = examinerId,
                        CreateAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDelete = false,
                        ClassId = null
                    };
                    _context.Examiners.Add(examiner);
                }
            }
        }
        else
        {
            // Tùy chọn cho từng lớp cho user chấm theo lớp cụ thể
            if (request.ExaminersForClass != null && request.ExaminersForClass.Any())
            {
                foreach (var item in request.ExaminersForClass)
                {
                    var classId = item.ClassId;
                    foreach (var exId in item.ExaminerIds)
                    {
                        var examiner = new Examiner
                        {
                            TestExamId = testExam.Id,
                            UserId = exId,
                            ClassId = classId,
                            CreateAt = DateTime.Now,
                            UpdateAt = DateTime.Now,
                            IsDelete = false
                        };
                        _context.Examiners.Add(examiner);
                    }
                }
            }
        }

        // 10) Lưu thay đổi lần cuối
        await _context.SaveChangesAsync();

        // 11) Trả về kết quả
        var classIds = _context.ClassTestExams
            .Where(x => x.TestExamId == testExam.Id && x.IsDelete == false)
            .Select(x => x.Id)
            .ToList();

        var examiners = _context.Examiners
            .Where(e => e.TestExamId == testExam.Id && e.IsDelete == false)
            .Select(e => new { e.Id, e.UserId })
            .ToList();
        var responseData = new
        {
            Id = testExam.Id,
            Topic = testExam.Topic,
            StartDate = testExam.StartDate,
            EndDate = testExam.EndDate,
            Duration = testExam.Duration?.ToString(),
            ClassIds = classIds,
            ExaminerIds = examiners.Select(e => e.UserId).ToList()
        };


        return new ApiResponse<object>(0, "Tạo lịch thi thành công", responseData);
    }

    public async Task<ApiResponse<object>> UpdateTestExamAsync(int id, UpdateTestExamRequest request)
    {
        // 1) Lấy đối tượng TestExam cần cập nhật
        var testExam = await _context.TestExams.FirstOrDefaultAsync(te => te.Id == id && te.IsDelete == false);
        if (testExam == null)
        {
            return new ApiResponse<object>(1, "Lịch thi không tồn tại", null);
        }

        // 2) Xác thực các giá trị: Học kỳ, Môn học, Loại kiểm tra nếu có giá trị mới truyền vào
        if (request.SemestersId.HasValue)
        {
            bool existsSemester = await _context.Semesters.AnyAsync(s => s.Id == request.SemestersId.Value);
            if (!existsSemester)
            {
                return new ApiResponse<object>(1, "Học kỳ không tồn tại trong DB", null);
            }
        }

        if (request.SubjectId.HasValue)
        {
            bool existsSubject = await _context.Subjects.AnyAsync(s => s.Id == request.SubjectId.Value);
            if (!existsSubject)
            {
                return new ApiResponse<object>(1, "Môn học không tồn tại trong DB", null);
            }
        }

        if (request.TestExamTypeId.HasValue)
        {
            bool existsType = await _context.TestExamTypes.AnyAsync(t => t.Id == request.TestExamTypeId.Value);
            if (!existsType)
            {
                return new ApiResponse<object>(1, "Loại kiểm tra không tồn tại trong DB", null);
            }
        }

        // 3) Map dữ liệu từ UpdateTestExamRequest sang testExam (cho phép cập nhật giá trị null)
        _mapper.Map(request, testExam);
        testExam.UpdateAt = DateTime.Now;
        // Lưu ý: Nếu mapping đã cấu hình để map DepartmentId, thì không cần gán lại
        // testExam.DepartmentId = request.DepartmentId; 

        // 4) Cập nhật lại danh sách ClassTestExam:
        // Xoá các bản ghi ClassTestExam hiện có của bài thi
        var existingClassTestExams =
            _context.ClassTestExams.Where(cte => cte.TestExamId == id && cte.IsDelete == false);
        _context.ClassTestExams.RemoveRange(existingClassTestExams);

        // Tính toán lại danh sách lớp dựa trên ClassOption & ClassIds
        var finalClassIds = new List<int>();
        if (request.ClassOption == "ALL")
        {
            finalClassIds = await _context.Classes
                .Where(c => c.IsDelete == false)
                .Select(c => c.Id)
                .ToListAsync();
        }
        else if (request.ClassOption == "TYPE")
        {
            if (request.SelectedClassTypeId.HasValue)
            {
                finalClassIds = await _context.Classes
                    .Where(c => c.ClassTypeId == request.SelectedClassTypeId.Value && c.IsDelete == false)
                    .Select(c => c.Id)
                    .ToListAsync();
            }
        }
        else // Giả sử trường hợp CUSTOM
        {
            if (request.ClassIds != null && request.ClassIds.Any())
            {
                finalClassIds = request.ClassIds;
            }
        }

        // Thêm các bản ghi ClassTestExam mới
        foreach (var classId in finalClassIds.Distinct())
        {
            var classTestExam = new ClassTestExam
            {
                TestExamId = testExam.Id,
                ClassId = classId,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                IsDelete = false
            };
            _context.ClassTestExams.Add(classTestExam);
        }

        // 5) Cập nhật lại phân công chấm thi (Examiner)
        // Xoá các bản ghi Examiner hiện có của bài thi
        var existingExaminers = _context.Examiners.Where(e => e.TestExamId == id && e.IsDelete == false);
        _context.Examiners.RemoveRange(existingExaminers);

        // Nếu ApplyExaminerForAllClasses = true, thêm examiner cho tất cả các lớp
        if (request.ApplyExaminerForAllClasses == true)
        {
            if (request.ExaminerIds != null && request.ExaminerIds.Any())
            {
                foreach (var examinerId in request.ExaminerIds)
                {
                    var examiner = new Examiner
                    {
                        TestExamId = testExam.Id,
                        UserId = examinerId,
                        CreateAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        IsDelete = false,
                        ClassId = null
                    };
                    _context.Examiners.Add(examiner);
                }
            }
        }
        else
        {
            // Xử lý phân công theo từng lớp nếu ApplyExaminerForAllClasses = false
            if (request.ExaminersForClass != null && request.ExaminersForClass.Any())
            {
                foreach (var item in request.ExaminersForClass)
                {
                    foreach (var exId in item.ExaminerIds)
                    {
                        var examiner = new Examiner
                        {
                            TestExamId = testExam.Id,
                            UserId = exId,
                            ClassId = item.ClassId,
                            CreateAt = DateTime.Now,
                            UpdateAt = DateTime.Now,
                            IsDelete = false
                        };
                        _context.Examiners.Add(examiner);
                    }
                }
            }
        }

        // 6) Lưu các thay đổi vào DB
        await _context.SaveChangesAsync();

        // 7) Chuẩn bị dữ liệu phản hồi
        var classIds = await _context.ClassTestExams
            .Where(x => x.TestExamId == testExam.Id && x.IsDelete == false)
            .Select(x => x.Id)
            .ToListAsync();
        var examiners = await _context.Examiners
            .Where(e => e.TestExamId == testExam.Id && e.IsDelete == false)
            .Select(e => new { e.Id, e.UserId })
            .ToListAsync();
        var responseData = new
        {
            Id = testExam.Id,
            Topic = testExam.Topic,
            StartDate = testExam.StartDate,
            EndDate = testExam.EndDate,
            Duration = testExam.Duration?.ToString(),
            ClassIds = classIds,
            ExaminerIds = examiners.Select(e => e.UserId).ToList()
        };

        return new ApiResponse<object>(0, "Cập nhật lịch thi thành công", responseData);
    }


    public async Task<ApiResponse<bool>> DeleteTestExamAsync(int id)
    {
        try
        {
            var testExam = await _context.TestExams.FindAsync(id);
            if (testExam == null)
                return new ApiResponse<bool>(1, "TestExam not found", false);
            if(testExam.IsDelete == true){
                return new ApiResponse<bool>(1, "TestExam is already deleted", false);
            }
            testExam.IsDelete = true;
            testExam.UpdateAt = DateTime.UtcNow.ToLocalTime();

            await _context.SaveChangesAsync();
            return new ApiResponse<bool>(0, "TestExam deleted successfully", true);
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>(1, $"Error deleting TestExam: {ex.Message}", false);
        }
    }

    public async Task<ApiResponse<IEnumerable<object>>> FilterClasses(int academicYearId, int departmentId)
    {
        // 1) Kiểm tra niên khóa có tồn tại không
        bool existYear = await _context.AcademicYears
            .AnyAsync(x => x.Id == academicYearId);
        if (!existYear)
        {
            return new ApiResponse<IEnumerable<object>>(
                1,
                "Niên khóa không tồn tại",
                null
            );
        }

        // 2) Kiểm tra department (khối) có tồn tại không
        bool existDept = await _context.Departments
            .AnyAsync(d => d.Id == departmentId);
        if (!existDept)
        {
            return new ApiResponse<IEnumerable<object>>(
                1,
                "Khối (department) không tồn tại",
                null
            );
        }

        // 3) Lọc danh sách lớp
        var result = await _context.Classes
            .Where
            (
                c => c.AcademicYearId == academicYearId
                     && c.DepartmentId == departmentId
                     && c.IsDelete == false
            )
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.ClassCode,
                c.StudentCount
            })
            .ToListAsync();

        // 4) Nếu không tìm thấy lớp nào, vẫn trả về success = 0 
        //    (tuỳ bạn có thể trả status=1 nếu muốn coi đó là "lỗi").
        if (result.Count == 0)
        {
            return new ApiResponse<IEnumerable<object>>(
                0,
                "Không tìm thấy lớp nào cho niên khóa & khối đã chọn",
                result
            );
        }

        // 5) Ngược lại, trả về thành công kèm danh sách
        return new ApiResponse<IEnumerable<object>>(
            0,
            "Lấy danh sách lớp thành công",
            result
        );
    }

    public async Task<ApiResponse<IEnumerable<object>>> GetAllAcademicYear()
    {
        try
        {
            var result = await _context.AcademicYears.ToListAsync();
            if (!result.Any())
            {
                return new ApiResponse<IEnumerable<object>>(1, "Không tìm thấy niên khóa nào", null);
            }

            var data = result.Where(c => c.IsDelete == false).Select(c => new
            {
                c.Id,
                c.StartDate,
                c.EndDate
            });

            return new ApiResponse<IEnumerable<object>>(0, "Lấy thông tin niên khóa thành công!", data);
        }
        catch (Exception e)
        {
            return new ApiResponse<IEnumerable<object>>(1, $"Đã xảy ra lỗi {e.Message}", null);
        }
    }

    public async Task<ApiResponse<IEnumerable<object>>> GetAllClasses()
    {
        try
        {
            var result = await _context.Departments.ToListAsync();
            if (!result.Any())
            {
                return new ApiResponse<IEnumerable<object>>(1, "Không tìm thấy khối (department) nào", null);
            }

            var data = result.Where(c => c.IsDelete == false).Select(c => new
            {
                c.Id,
                c.Name,
                c.UserId
            });
            return new ApiResponse<IEnumerable<object>>(0, "Lấy thông tin khối thành công!", data);
        }
        catch (Exception e)
        {
            return new ApiResponse<IEnumerable<object>>(1, $"Đã xảy ra lỗi {e.Message}", null);
        }
    }

    public async Task<ApiResponse<IEnumerable<object>>> GetAllAssignmentOfMarking()
    {
        try
        {
            var result = await _context.Users.ToListAsync();
            if (!result.Any())
            {
                return new ApiResponse<IEnumerable<object>>(1, $"Không tìn thấy (user) nào", null);
            }

            var data = result
                .Where(c => c.IsDelete == false)
                .Select(c => new
                {
                    c.Id,
                    c.Username,
                    c.FullName
                });

            return new ApiResponse<IEnumerable<object>>(0, $"Lấy thông tin người chấm thi thành công!", data);
        }
        catch (Exception e)
        {
            return new ApiResponse<IEnumerable<object>>(1, $"Đã xảy ra lỗi {e.Message}", null);
        }
    }
}