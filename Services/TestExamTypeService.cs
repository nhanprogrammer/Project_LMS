using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class TestExamTypeService : ITestExamTypeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TestExamTypeService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ApiResponse<PaginatedResponse<TestExamTypeResponse>>> GetAll(int pageNumber = 1, int pageSize = 10, string? keyword = null)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 10;
            }

            // Lấy tất cả bản ghi chưa bị xóa mềm từ database
            IQueryable<TestExamType> query = _context.TestExamTypes
                .Where(t => t.IsDelete == null || t.IsDelete == false);

            // Chuyển sang client-side evaluation
            var testExamTypes = await query.ToListAsync();

            // Nếu có keyword, lọc trên client
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalizedKeyword = keyword.Trim().ToLower().Normalize(NormalizationForm.FormD);
                testExamTypes = testExamTypes
                    .Where(t => t.PointTypeName != null &&
                                t.PointTypeName.ToLower().Normalize(NormalizationForm.FormD).Contains(normalizedKeyword))
                    .ToList();
            }

            // Tính toán phân trang trên client
            var totalItems = testExamTypes.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (pageNumber > totalPages && totalPages > 0)
            {
                pageNumber = totalPages;
            }

            // Phân trang trên client
            var pagedTestExamTypes = testExamTypes
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var testExamTypeResponses = _mapper.Map<List<TestExamTypeResponse>>(pagedTestExamTypes);

            var paginatedResponse = new PaginatedResponse<TestExamTypeResponse>
            {
                Items = testExamTypeResponses,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };

            return new ApiResponse<PaginatedResponse<TestExamTypeResponse>>(0, "Lấy tất cả loại bài kiểm tra thành công.")
            {
                Data = paginatedResponse
            };
        }
        public async Task<ApiResponse<List<int>>> GetCoefficients()
        {
            var coefficients = new List<int> { 1, 2, 3 };
            return new ApiResponse<List<int>>(0, "Lấy danh sách hệ số thành công.")
            {
                Data = coefficients
            };
        }

        public async Task<ApiResponse<TestExamTypeResponse>> Create(TestExamTypeRequest request)
        {
            var testExamType = _mapper.Map<TestExamType>(request);
            await _context.TestExamTypes.AddAsync(testExamType);
            await _context.SaveChangesAsync();
            var response = _mapper.Map<TestExamTypeResponse>(testExamType);
            return new ApiResponse<TestExamTypeResponse>(0, "Tạo loại bài kiểm tra thành công.")
            {
                Data = response
            };
        }

        public async Task<ApiResponse<TestExamTypeResponse>> Update(int id, TestExamTypeRequest request)
        {
            var testExamType = await _context.TestExamTypes.FindAsync(id);
            if (testExamType == null)
            {
                return new ApiResponse<TestExamTypeResponse>(1, "Loại bài kiểm tra không tồn tại.");
            }

            _mapper.Map(request, testExamType);
            await _context.SaveChangesAsync();
            var response = _mapper.Map<TestExamTypeResponse>(testExamType);
            return new ApiResponse<TestExamTypeResponse>(0, "Cập nhật loại bài kiểm tra thành công.")
            {
                Data = response
            };
        }

        public async Task<ApiResponse<TestExamTypeResponse>> Delete(int id)
        {
            var testExamType = await _context.TestExamTypes.FindAsync(id);
            if (testExamType == null)
            {
                return new ApiResponse<TestExamTypeResponse>(1, "Loại bài kiểm tra không tồn tại.");
            }
            if (testExamType.IsDelete == true)
            {
                return new ApiResponse<TestExamTypeResponse>(1, "Loại bài kiểm tra đã bị xóa.");
            }
            testExamType.IsDelete = true;
            await _context.SaveChangesAsync();
            return new ApiResponse<TestExamTypeResponse>(0, "Xóa loại bài kiểm tra thành công.");
        }

        public async Task<ApiResponse<TestExamTypeResponse>> Search(int id)
        {
            var testExamType = await _context.TestExamTypes.FindAsync(id);
            if (testExamType == null)
            {
                return new ApiResponse<TestExamTypeResponse>(1, "Không tìm thấy loại bài kiểm tra.");
            }

            var response = _mapper.Map<TestExamTypeResponse>(testExamType);
            return new ApiResponse<TestExamTypeResponse>(0, "Tìm thấy thành công.")
            {
                Data = response
            };
        }

        public async Task<ApiResponse<TestExamTypeResponse>> GetById(int id)
        {
            var testExamType = await _context.TestExamTypes
                .FirstOrDefaultAsync(t => t.Id == id && t.IsDelete == false);

            if (testExamType == null)
            {
                return new ApiResponse<TestExamTypeResponse>(1, "Không tìm thấy loại điểm với Id được cung cấp.", null);
            }

            var response = _mapper.Map<TestExamTypeResponse>(testExamType);
            return new ApiResponse<TestExamTypeResponse>(0, "Lấy loại điểm thành công!", response);
        }
    }
}