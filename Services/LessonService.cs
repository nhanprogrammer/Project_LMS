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

        public LessonService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PaginatedResponse<LessonResponse>>> GetLessonAsync(string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.Lessons
                    .Where(l => !l.IsDelete.HasValue || !l.IsDelete.Value);

                // Add search condition if keyword is provided
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    keyword = keyword.Trim().ToLower();
                    query = query.Where(l =>
                        (l.ClassLessonCode != null && l.ClassLessonCode.ToLower().Contains(keyword)) ||
                        (l.Topic != null && l.Topic.ToLower().Contains(keyword)) ||
                        (l.Description != null && l.Description.ToLower().Contains(keyword))
                    );
                }

                // Include related entities
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

                return new ApiResponse<PaginatedResponse<LessonResponse>>(0, "Success", paginatedResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedResponse<LessonResponse>>(1, $"Error getting lessons: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<LessonResponse>> GetLessonByIdAsync(int id)
        {
            var lesson = await _context.Lessons
                .Include(l => l.TeachingAssignment)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id && (!l.IsDelete.HasValue || !l.IsDelete.Value));

            if (lesson == null)
                return new ApiResponse<LessonResponse>(1, "Lesson not found", null);

            var response = _mapper.Map<LessonResponse>(lesson);
            return new ApiResponse<LessonResponse>(0, "Success", response);
        }

        public async Task<ApiResponse<LessonResponse>> CreateLessonAsync(CreateLessonRequest request)
        {
            try
            {
                if (request == null)
                    return new ApiResponse<LessonResponse>(1, "Invalid request data", null);

                if (!string.IsNullOrEmpty(request.ClassLessonCode))
                {
                    var existingLesson = await _context.Lessons
                        .FirstOrDefaultAsync(l => l.ClassLessonCode == request.ClassLessonCode &&
                                                (!l.IsDelete.HasValue || !l.IsDelete.Value));
                    if (existingLesson != null)
                    {
                        return new ApiResponse<LessonResponse>(1, "Class lesson code already exists", null);
                    }
                }

                // Validate TeachingAssignment exists
                var teachingAssignment = await _context.TeachingAssignments
                    .FirstOrDefaultAsync(ta => ta.Id == request.TeachingAssignmentId);

                if (teachingAssignment == null)
                    return new ApiResponse<LessonResponse>(1, "Teaching Assignment not found", null);

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

                if (user == null)
                    return new ApiResponse<LessonResponse>(1, "User not found", null);

                var lesson = _mapper.Map<Lesson>(request);

                // Hash the password if provided
                if (!string.IsNullOrEmpty(request.PaswordLeassons))
                {
                    lesson.PaswordLeassons = BCrypt.Net.BCrypt.HashPassword(request.PaswordLeassons);
                }

                lesson.CreateAt = DateTime.UtcNow.ToLocalTime();
                lesson.IsDelete = false;

                _context.Lessons.Add(lesson);
                await _context.SaveChangesAsync();

                await _context.Entry(lesson)
                    .Reference(l => l.TeachingAssignment)
                    .LoadAsync();

                var response = _mapper.Map<LessonResponse>(lesson);
                return new ApiResponse<LessonResponse>(0, "Lesson created successfully", response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<LessonResponse>(1, $"Error creating lesson: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<LessonResponse>> UpdateLessonAsync(CreateLessonRequest request)
        {
            try
            {
                var existingLesson = await _context.Lessons
                    .Include(l => l.TeachingAssignment)
                    .FirstOrDefaultAsync(l => l.Id == request.Id && (!l.IsDelete.HasValue || !l.IsDelete.Value));

                if (existingLesson == null)
                    return new ApiResponse<LessonResponse>(1, "Lesson not found", null);

                if (!string.IsNullOrEmpty(request.ClassLessonCode) &&
                           request.ClassLessonCode != existingLesson.ClassLessonCode)
                {
                    var duplicateLesson = await _context.Lessons
                        .FirstOrDefaultAsync(l => l.ClassLessonCode == request.ClassLessonCode &&
                                                l.Id != request.Id &&
                                                (!l.IsDelete.HasValue || !l.IsDelete.Value));
                    if (duplicateLesson != null)
                    {
                        return new ApiResponse<LessonResponse>(1, "Class lesson code already exists", null);
                    }
                }
                // Validate TeachingAssignment exists if it's being updated
                if (request.TeachingAssignmentId != existingLesson.TeachingAssignmentId)
                {
                    var teachingAssignment = await _context.TeachingAssignments
                        .FirstOrDefaultAsync(ta => ta.Id == request.TeachingAssignmentId);

                    if (teachingAssignment == null)
                        return new ApiResponse<LessonResponse>(1, "Teaching Assignment not found", null);

                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

                    if (user == null)
                        return new ApiResponse<LessonResponse>(1, "User not found", null);
                }

                _mapper.Map(request, existingLesson);
                existingLesson.UpdateAt = DateTime.UtcNow.ToLocalTime();

                await _context.SaveChangesAsync();

                var response = _mapper.Map<LessonResponse>(existingLesson);
                return new ApiResponse<LessonResponse>(0, "Lesson updated successfully", response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<LessonResponse>(1, $"Error updating lesson: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<bool>> DeleteLessonAsync(int id)
        {
            try
            {
                var lesson = await _context.Lessons.FindAsync(id);
                if (lesson == null || lesson.IsDelete == true)
                    return new ApiResponse<bool>(1, "Lesson not found", false);

                lesson.IsDelete = true;
                lesson.UpdateAt = DateTime.UtcNow.ToLocalTime();

                await _context.SaveChangesAsync();
                return new ApiResponse<bool>(0, "Lesson deleted successfully", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(1, $"Error deleting lesson: {ex.Message}", false);
            }
        }

        public async Task<ApiResponse<bool>> DeleteMultipleLessonsAsync(List<int> ids)
        {
            try
            {
                var lessons = await _context.Lessons
                    .Where(l => ids.Contains(l.Id) && (!l.IsDelete.HasValue || !l.IsDelete.Value))
                    .ToListAsync();

                if (!lessons.Any())
                    return new ApiResponse<bool>(1, "No lessons found to delete", false);

                foreach (var lesson in lessons)
                {
                    lesson.IsDelete = true;
                    lesson.UpdateAt = DateTime.UtcNow.ToLocalTime();
                }

                await _context.SaveChangesAsync();
                return new ApiResponse<bool>(0, $"Successfully deleted {lessons.Count} lessons", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(1, $"Error deleting lessons: {ex.Message}", false);
            }
        }
    }
}
