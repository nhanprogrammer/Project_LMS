using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Project_LMS.Repositories;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Helpers;
using Project_LMS.Utils;

namespace Project_LMS.Services
{
    public class LessonService : ILessonService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public LessonService(ApplicationDbContext context, IMapper mapper, IAuthService authService)
        {
            _context = context;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<ApiResponse<PaginatedResponse<LessonResponse>>> GetLessonAsync(string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<PaginatedResponse<LessonResponse>>(1, "Không có quyền truy cập", null);

                var query = _context.Lessons
                    .Where(l => !l.IsDelete.HasValue || !l.IsDelete.Value);

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    keyword = keyword.Trim().ToLower();
                    query = query.Where(l =>
                        (l.ClassLessonCode != null && l.ClassLessonCode.ToLower().Contains(keyword)) ||
                        (l.Topic != null && l.Topic.ToLower().Contains(keyword)) ||
                        (l.Description != null && l.Description.ToLower().Contains(keyword))
                    );
                }

                query = query
                    .Include(l => l.TeachingAssignment)
                        .ThenInclude(ta => ta.Subject)
                    .Include(l => l.User)
                    .OrderByDescending(l => l.Id);

                var totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var lessons = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var lessonResponses = _mapper.Map<List<LessonResponse>>(lessons);

                var paginatedResponse = new PaginatedResponse<LessonResponse>
                {
                    Items = lessonResponses,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    HasPreviousPage = pageNumber > 1,
                    HasNextPage = pageNumber < totalPages
                };

                return new ApiResponse<PaginatedResponse<LessonResponse>>(0, "Lấy danh sách bài học thành công", paginatedResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedResponse<LessonResponse>>(1, $"Lỗi khi lấy danh sách bài học: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<LessonResponse>> GetLessonByIdAsync(int id)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<LessonResponse>(1, "Không có quyền truy cập", null);

                var lesson = await _context.Lessons
                    .Include(l => l.TeachingAssignment)
                    .Include(l => l.User)
                    .FirstOrDefaultAsync(l => l.Id == id && (!l.IsDelete.HasValue || !l.IsDelete.Value));

                if (lesson == null)
                    return new ApiResponse<LessonResponse>(1, "Không tìm thấy bài học", null);

                var response = _mapper.Map<LessonResponse>(lesson);
                return new ApiResponse<LessonResponse>(0, "Lấy thông tin bài học thành công", response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<LessonResponse>(1, $"Lỗi khi lấy thông tin bài học: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<LessonResponse>> CreateLessonAsync(CreateLessonRequest request)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<LessonResponse>(1, "Không có quyền truy cập", null);

                if (request == null)
                    return new ApiResponse<LessonResponse>(1, "Dữ liệu không hợp lệ", null);

                // Validate ClassLessonCode uniqueness
                if (!string.IsNullOrEmpty(request.ClassLessonCode))
                {
                    var existingLesson = await _context.Lessons
                        .FirstOrDefaultAsync(l => l.ClassLessonCode == request.ClassLessonCode &&
                                                (!l.IsDelete.HasValue || !l.IsDelete.Value));
                    if (existingLesson != null)
                    {
                        return new ApiResponse<LessonResponse>(1, "Mã bài học đã tồn tại", null);
                    }
                }

                // Get TeachingAssignment with its existing lessons
                var teachingAssignment = await _context.TeachingAssignments
                    .Include(ta => ta.Lessons.Where(l => !l.IsDelete.HasValue || !l.IsDelete.Value))
                    .FirstOrDefaultAsync(ta => ta.Id == request.TeachingAssignmentId);

                if (teachingAssignment == null)
                    return new ApiResponse<LessonResponse>(1, "Không tìm thấy phân công giảng dạy", null);

                // Validate lesson time within TeachingAssignment time range
                if (request.StartDate < teachingAssignment.StartDate || request.EndDate > teachingAssignment.EndDate)
                {
                    return new ApiResponse<LessonResponse>(1,
                        $"Thời gian buổi học phải nằm trong khoảng thời gian của phân công giảng dạy " +
                        $"({teachingAssignment.StartDate:dd/MM/yyyy HH:mm} - {teachingAssignment.EndDate:dd/MM/yyyy HH:mm})", null);
                }

                // Validate lesson time overlap with other lessons
                var hasOverlap = teachingAssignment.Lessons.Any(l =>
                    // Check if new lesson's time range overlaps with any existing lesson
                    (request.StartDate >= l.StartDate && request.StartDate <= l.EndDate) || // New start within existing
                    (request.EndDate >= l.StartDate && request.EndDate <= l.EndDate) || // New end within existing
                    (request.StartDate <= l.StartDate && request.EndDate >= l.EndDate) // New lesson completely contains existing
                );

                if (hasOverlap)
                {
                    return new ApiResponse<LessonResponse>(1,
                        "Thời gian buổi học trùng với buổi học khác trong cùng phân công giảng dạy", null);
                }

                // Create new lesson
                var lesson = _mapper.Map<Lesson>(request);

                if (!string.IsNullOrEmpty(request.PaswordLeassons))
                {
                    lesson.PaswordLeassons = BCrypt.Net.BCrypt.HashPassword(request.PaswordLeassons);
                }

                lesson.CreateAt = DateTime.UtcNow.ToLocalTime();
                lesson.IsDelete = false;
                lesson.UserCreate = user.Id;

                _context.Lessons.Add(lesson);
                await _context.SaveChangesAsync();

                await _context.Entry(lesson)
                    .Reference(l => l.TeachingAssignment)
                    .LoadAsync();

                var response = _mapper.Map<LessonResponse>(lesson);
                return new ApiResponse<LessonResponse>(0, "Tạo bài học thành công", response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<LessonResponse>(1, $"Lỗi khi tạo bài học: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<LessonResponse>> UpdateLessonAsync(CreateLessonRequest request)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<LessonResponse>(1, "Không có quyền truy cập", null);

                var existingLesson = await _context.Lessons
                    .Include(l => l.TeachingAssignment)
                    .FirstOrDefaultAsync(l => l.Id == request.Id && (!l.IsDelete.HasValue || !l.IsDelete.Value));

                if (existingLesson == null)
                    return new ApiResponse<LessonResponse>(1, "Không tìm thấy bài học", null);

                // Validate ClassLessonCode uniqueness
                if (!string.IsNullOrEmpty(request.ClassLessonCode) &&
                           request.ClassLessonCode != existingLesson.ClassLessonCode)
                {
                    var duplicateLesson = await _context.Lessons
                        .FirstOrDefaultAsync(l => l.ClassLessonCode == request.ClassLessonCode &&
                                                l.Id != request.Id &&
                                                (!l.IsDelete.HasValue || !l.IsDelete.Value));
                    if (duplicateLesson != null)
                    {
                        return new ApiResponse<LessonResponse>(1, "Mã bài học đã tồn tại", null);
                    }
                }

                // Get TeachingAssignment with its existing lessons
                var teachingAssignment = await _context.TeachingAssignments
                    .Include(ta => ta.Lessons.Where(l => !l.IsDelete.HasValue || !l.IsDelete.Value))
                    .FirstOrDefaultAsync(ta => ta.Id == request.TeachingAssignmentId);

                if (teachingAssignment == null)
                    return new ApiResponse<LessonResponse>(1, "Không tìm thấy phân công giảng dạy", null);

                // Validate lesson time within TeachingAssignment time range
                if (request.StartDate < teachingAssignment.StartDate || request.EndDate > teachingAssignment.EndDate)
                {
                    return new ApiResponse<LessonResponse>(1,
                        $"Thời gian buổi học phải nằm trong khoảng thời gian của phân công giảng dạy " +
                        $"({teachingAssignment.StartDate:dd/MM/yyyy HH:mm} - {teachingAssignment.EndDate:dd/MM/yyyy HH:mm})", null);
                }

                // Validate lesson time overlap with other lessons (excluding the current lesson)
                var hasOverlap = teachingAssignment.Lessons
                    .Where(l => l.Id != request.Id) // Exclude current lesson
                    .Any(l =>
                        (request.StartDate >= l.StartDate && request.StartDate <= l.EndDate) || // New start within existing
                        (request.EndDate >= l.StartDate && request.EndDate <= l.EndDate) || // New end within existing
                        (request.StartDate <= l.StartDate && request.EndDate >= l.EndDate) // New lesson completely contains existing
                    );

                if (hasOverlap)
                {
                    return new ApiResponse<LessonResponse>(1,
                        "Thời gian buổi học trùng với buổi học khác trong cùng phân công giảng dạy", null);
                }

                // Update lesson
                _mapper.Map(request, existingLesson);

                // Hash password if provided
                if (!string.IsNullOrEmpty(request.PaswordLeassons))
                {
                    existingLesson.PaswordLeassons = BCrypt.Net.BCrypt.HashPassword(request.PaswordLeassons);
                }

                existingLesson.UpdateAt = DateTime.UtcNow.ToLocalTime();
                existingLesson.UserUpdate = user.Id;

                await _context.SaveChangesAsync();

                var response = _mapper.Map<LessonResponse>(existingLesson);
                return new ApiResponse<LessonResponse>(0, "Cập nhật bài học thành công", response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<LessonResponse>(1, $"Lỗi khi cập nhật bài học: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<bool>> DeleteLessonAsync(int id)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<bool>(1, "Không có quyền truy cập", false);

                var lesson = await _context.Lessons.FindAsync(id);
                if (lesson == null || lesson.IsDelete == true)
                    return new ApiResponse<bool>(1, "Không tìm thấy bài học", false);

                lesson.IsDelete = true;
                lesson.UpdateAt = DateTime.UtcNow.ToLocalTime();
                lesson.UserUpdate = user.Id;

                await _context.SaveChangesAsync();
                return new ApiResponse<bool>(0, "Xóa bài học thành công", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(1, $"Lỗi khi xóa bài học: {ex.Message}", false);
            }
        }

        public async Task<ApiResponse<bool>> DeleteMultipleLessonsAsync(List<int> ids)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<bool>(1, "Không có quyền truy cập", false);

                var lessons = await _context.Lessons
                    .Where(l => ids.Contains(l.Id) && (!l.IsDelete.HasValue || !l.IsDelete.Value))
                    .ToListAsync();

                if (!lessons.Any())
                    return new ApiResponse<bool>(1, "Không tìm thấy bài học để xóa", false);

                foreach (var lesson in lessons)
                {
                    lesson.IsDelete = true;
                    lesson.UpdateAt = DateTime.UtcNow.ToLocalTime();
                    lesson.UserUpdate = user.Id;
                }

                await _context.SaveChangesAsync();
                return new ApiResponse<bool>(0, $"Đã xóa thành công {lessons.Count} bài học", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(1, $"Lỗi khi xóa bài học: {ex.Message}", false);
            }
        }
    }
}
