using System.Net.Mail;
using System.Net.WebSockets;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;


namespace Project_LMS.Services;

public class TestExamService : ITestExamService
{
    private readonly ITestExamRepository _testExamRepository;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public TestExamService(ITestExamRepository testExamRepository, ApplicationDbContext context, IMapper mapper)
    {
        _testExamRepository = testExamRepository;
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PaginatedResponse<TestExamResponse>>> GetAllTestExamsAsync(string? keyword,
        int? pageNumber, int? pageSize, string? sortDirection)
    {
        if (pageNumber.HasValue && pageNumber <= 0)
        {
            return new ApiResponse<PaginatedResponse<TestExamResponse>>(
                1,
                "Giá trị pageNumber phải lớn hơn 0",
                null
            );
        }

        if (pageSize.HasValue && pageSize <= 0)
        {
            return new ApiResponse<PaginatedResponse<TestExamResponse>>(
                1,
                "Giá trị pageSize phải lớn hơn 0",
                null
            );
        }

        if (!string.IsNullOrEmpty(sortDirection) &&
            !sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase) &&
            !sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
        {
            return new ApiResponse<PaginatedResponse<TestExamResponse>>(
                1,
                "Giá trị sortDirection phải là 'asc' hoặc 'desc'",
                null
            );
        }

        try
        {
            // 1. Xác định pageNumber, pageSize mặc định
            var currentPage = pageNumber ?? 1;
            var currentPageSize = pageSize ?? 10;

            // 2. Lấy danh sách test exams
            var testExams = await _testExamRepository.GetAllAsync();
            var testExamQuery = testExams.AsQueryable();

            // 3. Nếu không nhập sortDirection, mặc định là "asc"
            sortDirection ??= "asc";

            testExamQuery = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
                ? testExamQuery.OrderByDescending(te => te.Id).ThenByDescending(te => te.Id)
                : testExamQuery.OrderBy(te => te.Id).ThenByDescending(te => te.Id);

            // 5. Tính toán tổng số dòng, số trang
            var totalItems = testExamQuery.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / currentPageSize);

            // 6. Phân trang
            var pagedTestExams = testExamQuery
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();

            // 7. Map sang DTO
            var mappedData = _mapper.Map<List<TestExamResponse>>(pagedTestExams);

            // 8. Tạo đối tượng phân trang
            var paginatedResponse = new PaginatedResponse<TestExamResponse>
            {
                Items = mappedData,
                PageNumber = currentPage,
                PageSize = currentPageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = currentPage > 1,
                HasNextPage = currentPage < totalPages
            };

            // 9. Trả về
            return new ApiResponse<PaginatedResponse<TestExamResponse>>(
                0,
                "Lấy dữ liệu thành công",
                paginatedResponse
            );
        }
        catch (Exception ex)
        {
            return new ApiResponse<PaginatedResponse<TestExamResponse>>(
                1,
                $"Lỗi: {ex.Message}",
                null
            );
        }
    }


    public async Task<ApiResponse<TestExamResponse>> GetTestExamByIdAsync(int id)
    {
        var testExams = await _testExamRepository.GetByIdAsync(id);
        
        if (testExams == null)
        {
            return new ApiResponse<TestExamResponse>(1, "Nhóm môn học không tồn tại", null);
        }

        var testExamResponse = new TestExamResponse()
        {
            Id = testExams.Id,
            SubjectName = testExams.Subject.SubjectName,
            StatusExam = testExams.TestExamType.PointTypeName,
            Duration = testExams.Duration,
            Semester = testExams.Semesters.Name,
            StartDate = testExams.StartDate,
            DepartmentName = testExams.Department.Name,
            ClassList =  string.Join(", ", testExams.ClassTestExams.Select(e => e.Class.Name)),
            Examiner =  string.Join("  ", testExams.Examiners.Select(e => e.User.FullName))
        };

        return new ApiResponse<TestExamResponse>(0, "Lấy dữ liệu thành công", testExamResponse);
    }

    public Task<ApiResponse<TestExamResponse>> CreateTestExamAsync(TestExamRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<TestExamResponse>> UpdateTestExamAsync(int id, TestExamRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<bool>> DeleteTestExamAsync(int id)
    {
        throw new NotImplementedException();
    }
}