using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class SubjectTypeService : ISubjectTypeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public SubjectTypeService(
            ApplicationDbContext context,
            IMapper mapper,
            IAuthService authService)
        {
            _context = context;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<ApiResponse<PaginatedResponse<SubjectTypeResponse>>> GetAllSubjectTypesAsync(string? keyword, int pageNumber, int pageSize)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<PaginatedResponse<SubjectTypeResponse>>(1, "Không có quyền truy cập", null);

                var query = _context.SubjectTypes
                    .Where(st => !(st.IsDelete ?? false));

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    keyword = keyword.Trim().ToLower();
                    query = query.Where(st =>
                        (st.Name != null && st.Name.ToLower().Contains(keyword)) ||
                        (st.Note != null && st.Note.ToLower().Contains(keyword))
                    );
                }

                query = query.OrderByDescending(st => st.Id);

                var totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var subjectTypes = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var responses = _mapper.Map<List<SubjectTypeResponse>>(subjectTypes);

                var paginatedResponse = new PaginatedResponse<SubjectTypeResponse>
                {
                    Items = responses,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    HasPreviousPage = pageNumber > 1,
                    HasNextPage = pageNumber < totalPages
                };

                return new ApiResponse<PaginatedResponse<SubjectTypeResponse>>(0, "Lấy danh sách loại môn học thành công", paginatedResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedResponse<SubjectTypeResponse>>(1, $"Lỗi khi lấy danh sách loại môn học: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<SubjectTypeResponse>> GetSubjectTypeByIdAsync(int id)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<SubjectTypeResponse>(1, "Không có quyền truy cập", null);

                var subjectType = await _context.SubjectTypes
                    .FirstOrDefaultAsync(st => st.Id == id && !(st.IsDelete ?? false));

                if (subjectType == null)
                    return new ApiResponse<SubjectTypeResponse>(1, "Không tìm thấy loại môn học", null);

                var response = _mapper.Map<SubjectTypeResponse>(subjectType);
                return new ApiResponse<SubjectTypeResponse>(0, "Lấy thông tin loại môn học thành công", response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<SubjectTypeResponse>(1, $"Lỗi khi lấy thông tin loại môn học: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<SubjectTypeResponse>> CreateSubjectTypeAsync(SubjectTypeRequest request)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<SubjectTypeResponse>(1, "Không có quyền truy cập", null);

                if (request == null || string.IsNullOrEmpty(request.Name))
                    return new ApiResponse<SubjectTypeResponse>(1, "Tên loại môn học không được để trống", null);

                // Kiểm tra tên loại môn học đã tồn tại chưa
                var existingSubjectType = await _context.SubjectTypes
                    .FirstOrDefaultAsync(st => st.Name != null &&
                        st.Name.ToLower() == request.Name.ToLower() &&
                        (!st.IsDelete.HasValue || !st.IsDelete.Value));

                if (existingSubjectType != null)
                    return new ApiResponse<SubjectTypeResponse>(1, "Tên loại môn học đã tồn tại", null);

                var subjectType = _mapper.Map<SubjectType>(request);
                subjectType.CreateAt = DateTime.UtcNow.ToLocalTime();
                subjectType.IsDelete = false;
                subjectType.UserCreate = user.Id;

                await _context.SubjectTypes.AddAsync(subjectType);
                await _context.SaveChangesAsync();

                var response = _mapper.Map<SubjectTypeResponse>(subjectType);
                return new ApiResponse<SubjectTypeResponse>(0, "Tạo loại môn học thành công", response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<SubjectTypeResponse>(1, $"Lỗi khi tạo loại môn học: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<SubjectTypeResponse>> UpdateSubjectTypeAsync(SubjectTypeRequest request)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<SubjectTypeResponse>(1, "Không có quyền truy cập", null);

                if (request == null || string.IsNullOrEmpty(request.Name))
                    return new ApiResponse<SubjectTypeResponse>(1, "Tên loại môn học không được để trống", null);

                var existingSubjectType = await _context.SubjectTypes
                    .FirstOrDefaultAsync(st => st.Id == request.Id && !(st.IsDelete ?? false));

                if (existingSubjectType == null)
                    return new ApiResponse<SubjectTypeResponse>(1, "Không tìm thấy loại môn học", null);

                // Kiểm tra tên loại môn học đã tồn tại chưa (loại trừ chính nó)
                var duplicateName = await _context.SubjectTypes
                    .FirstOrDefaultAsync(st => st.Id != request.Id
                        && st.Name != null
                        && st.Name.ToLower() == request.Name.ToLower()
                        && (!st.IsDelete.HasValue || !st.IsDelete.Value));

                if (duplicateName != null)
                    return new ApiResponse<SubjectTypeResponse>(1, "Tên loại môn học đã tồn tại", null);

                _mapper.Map(request, existingSubjectType);
                existingSubjectType.UpdateAt = DateTime.UtcNow.ToLocalTime();
                existingSubjectType.UserUpdate = user.Id;

                await _context.SaveChangesAsync();

                var response = _mapper.Map<SubjectTypeResponse>(existingSubjectType);
                return new ApiResponse<SubjectTypeResponse>(0, "Cập nhật loại môn học thành công", response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<SubjectTypeResponse>(1, $"Lỗi khi cập nhật loại môn học: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<bool>> DeleteSubjectTypeAsync(List<int> ids)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<bool>(1, "Không có quyền truy cập", false);

                var subjectTypes = await _context.SubjectTypes
                    .Where(st => ids.Contains(st.Id) && (!st.IsDelete.HasValue || !st.IsDelete.Value))
                    .ToListAsync();

                if (!subjectTypes.Any())
                {
                    return new ApiResponse<bool>(1, "Không tìm thấy loại môn học để xóa", false);
                }

                foreach (var subjectType in subjectTypes)
                {
                    subjectType.IsDelete = true;
                    subjectType.UpdateAt = DateTime.UtcNow.ToLocalTime();
                    subjectType.UserUpdate = user.Id;
                }

                await _context.SaveChangesAsync();
                return new ApiResponse<bool>(0, $"Đã xóa thành công {subjectTypes.Count} loại môn học", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(1, $"Lỗi khi xóa loại môn học: {ex.Message}", false);
            }
        }
    }
}