using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_LMS.Services
{
    public class ClassTypeService : IClassTypeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public ClassTypeService(ApplicationDbContext context, IMapper mapper, IAuthService authService)
        {
            _context = context;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<ApiResponse<PaginatedResponse<ClassTypeResponse>>> GetAllClassTypesAsync(string? keyword, int pageNumber, int pageSize)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<PaginatedResponse<ClassTypeResponse>>(1, "Không có quyền truy cập", null);

                var query = _context.ClassTypes
                    .Where(ct => !(ct.IsDelete ?? false));

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    keyword = keyword.Trim().ToLower();
                    query = query.Where(ct =>
                        (ct.Name != null && ct.Name.ToLower().Contains(keyword)) ||
                        (ct.Note != null && ct.Note.ToLower().Contains(keyword))
                    );
                }

                query = query.OrderByDescending(ct => ct.Id);

                var totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var classTypes = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var responses = _mapper.Map<List<ClassTypeResponse>>(classTypes);

                var paginatedResponse = new PaginatedResponse<ClassTypeResponse>
                {
                    Items = responses,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    HasPreviousPage = pageNumber > 1,
                    HasNextPage = pageNumber < totalPages
                };

                return new ApiResponse<PaginatedResponse<ClassTypeResponse>>(0, "Lấy danh sách loại lớp học thành công", paginatedResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedResponse<ClassTypeResponse>>(1, $"Lỗi khi lấy danh sách loại lớp học: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<ClassTypeResponse>> GetClassTypeByIdAsync(int id)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<ClassTypeResponse>(1, "Không có quyền truy cập", null);

                var classType = await _context.ClassTypes
                    .FirstOrDefaultAsync(ct => ct.Id == id && !(ct.IsDelete ?? false));

                if (classType == null)
                    return new ApiResponse<ClassTypeResponse>(1, "Không tìm thấy loại lớp học", null);

                var response = _mapper.Map<ClassTypeResponse>(classType);
                return new ApiResponse<ClassTypeResponse>(0, "Lấy thông tin loại lớp học thành công", response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ClassTypeResponse>(1, $"Lỗi khi lấy thông tin loại lớp học: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<ClassTypeResponse>> CreateClassTypeAsync(ClassTypeRequest request)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<ClassTypeResponse>(1, "Không có quyền truy cập", null);

                var classType = _mapper.Map<ClassType>(request);
                classType.CreateAt = DateTime.UtcNow.ToLocalTime();
                classType.IsDelete = false;
                classType.UserCreate = user.Id;

                await _context.ClassTypes.AddAsync(classType);
                await _context.SaveChangesAsync();

                var response = _mapper.Map<ClassTypeResponse>(classType);
                return new ApiResponse<ClassTypeResponse>(0, "Tạo loại lớp học thành công", response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ClassTypeResponse>(1, $"Lỗi khi tạo loại lớp học: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<ClassTypeResponse>> UpdateClassTypeAsync(ClassTypeRequest request)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<ClassTypeResponse>(1, "Không có quyền truy cập", null);

                var existingClassType = await _context.ClassTypes
                    .FirstOrDefaultAsync(ct => ct.Id == request.id && !(ct.IsDelete ?? false));

                if (existingClassType == null)
                    return new ApiResponse<ClassTypeResponse>(1, "Không tìm thấy loại lớp học", null);

                _mapper.Map(request, existingClassType);
                existingClassType.UpdateAt = DateTime.UtcNow.ToLocalTime();
                existingClassType.UserUpdate = user.Id;

                await _context.SaveChangesAsync();

                var response = _mapper.Map<ClassTypeResponse>(existingClassType);
                return new ApiResponse<ClassTypeResponse>(0, "Cập nhật loại lớp học thành công", response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ClassTypeResponse>(1, $"Lỗi khi cập nhật loại lớp học: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<bool>> DeleteClassTypeAsync(List<int> ids)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<bool>(1, "Không có quyền truy cập", false);

                var classTypes = await _context.ClassTypes
                    .Where(c => ids.Contains(c.Id) && (!c.IsDelete.HasValue || !c.IsDelete.Value))
                    .ToListAsync();

                if (!classTypes.Any())
                    return new ApiResponse<bool>(1, "Không tìm thấy loại lớp học để xóa", false);

                foreach (var classType in classTypes)
                {
                    classType.IsDelete = true;
                    classType.UpdateAt = DateTime.UtcNow.ToLocalTime();
                    classType.UserUpdate = user.Id;
                }

                await _context.SaveChangesAsync();
                return new ApiResponse<bool>(0, $"Đã xóa thành công {classTypes.Count} loại lớp học", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(1, $"Lỗi khi xóa loại lớp học: {ex.Message}", false);
            }
        }
        public async Task<List<ClassTypeDropdownResponse>> GetClassTypeDropdownAsync()
        {
            return await _context.ClassTypes
                .Where(ct => !(ct.IsDelete ?? false) && (ct.Status ?? false))
                .Select(ct => new ClassTypeDropdownResponse
                {
                    Id = ct.Id,
                    Name = ct.Name ?? string.Empty
                })
                .ToListAsync();
        }
    }
}
