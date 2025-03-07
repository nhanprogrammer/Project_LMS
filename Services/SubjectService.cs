using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SubjectService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PaginatedResponse<SubjectResponse>>> GetAllSubjectsAsync(string? keyword, int pageNumber, int pageSize)
        {
            var query = _context.Subjects
                .Where(s => !s.IsDelete.HasValue || !s.IsDelete.Value);

            // Add search condition if keyword is provided
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim().ToLower();
                query = query.Where(s => 
                    (s.SubjectCode != null && s.SubjectCode.ToLower().Contains(keyword)) || 
                    (s.SubjectName != null && s.SubjectName.ToLower().Contains(keyword))
                );
            }

            // Include related entities
            query = query
                .Include(s => s.SubjectType)
                .Include(s => s.SubjectGroup)
                .Include(s => s.TeachingAssignment);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var subjects = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var subjectResponses = _mapper.Map<List<SubjectResponse>>(subjects);

            var paginatedResponse = new PaginatedResponse<SubjectResponse>
            {
                Items = subjectResponses,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };

            return new ApiResponse<PaginatedResponse<SubjectResponse>>(0, "Success", paginatedResponse);
        }

        public async Task<ApiResponse<SubjectResponse>> GetSubjectByIdAsync(int id)
        {
            var subject = await _context.Subjects
                .Include(s => s.SubjectType)
                .Include(s => s.SubjectGroup)
                .Include(s => s.TeachingAssignment)
                .FirstOrDefaultAsync(s => s.Id == id && (!s.IsDelete.HasValue || !s.IsDelete.Value));

            if (subject == null)
                return new ApiResponse<SubjectResponse>(1, "Subject not found", null);

            var response = _mapper.Map<SubjectResponse>(subject);
            return new ApiResponse<SubjectResponse>(0, "Success", response);
        }

        public async Task<ApiResponse<SubjectResponse>> CreateSubjectAsync(SubjectRequest request)
        {
            try
            {
                var subject = _mapper.Map<Subject>(request);
                // Convert UTC to local time for PostgreSQL timestamp without time zone
                subject.CreateAt = DateTime.UtcNow.ToLocalTime();
                subject.IsDelete = false;

                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();

                var response = _mapper.Map<SubjectResponse>(subject);
                return new ApiResponse<SubjectResponse>(0, "Subject created successfully", response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<SubjectResponse>(1, $"Error creating subject: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<SubjectResponse>> UpdateSubjectAsync(int id, SubjectRequest request)
        {
            try
            {
                var subject = await _context.Subjects.FindAsync(id);
                if (subject == null || subject.IsDelete == true)
                    return new ApiResponse<SubjectResponse>(1, "Subject not found", null);

                _mapper.Map(request, subject);
                // Convert UTC to local time for PostgreSQL timestamp without time zone
                subject.UpdateAt = DateTime.UtcNow.ToLocalTime();

                await _context.SaveChangesAsync();

                var response = _mapper.Map<SubjectResponse>(subject);
                return new ApiResponse<SubjectResponse>(0, "Subject updated successfully", response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<SubjectResponse>(1, $"Error updating subject: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<bool>> DeleteSubjectAsync(int id)
        {
            try 
            {
                var subject = await _context.Subjects.FindAsync(id);
                if (subject == null || subject.IsDelete == true)
                    return new ApiResponse<bool>(1, "Subject not found", false);

                subject.IsDelete = true;
                // Convert UTC to local time for PostgreSQL timestamp without time zone
                subject.UpdateAt = DateTime.UtcNow.ToLocalTime();
                
                await _context.SaveChangesAsync();
                return new ApiResponse<bool>(0, "Subject deleted successfully", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(1, $"Error deleting subject: {ex.Message}", false);
            }
        }
    }
}