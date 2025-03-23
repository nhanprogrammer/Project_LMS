using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class SubjectTypeService : ISubjectTypeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SubjectTypeService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PaginatedResponse<SubjectTypeResponse>>> GetAllSubjectTypesAsync(string? keyword, int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.SubjectTypes
                    .Where(st => !(st.IsDelete ?? false));

                // Add search condition if keyword is provided
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    keyword = keyword.Trim().ToLower();
                    query = query.Where(st =>
                        (st.Name != null && st.Name.ToLower().Contains(keyword)) ||
                        (st.Note != null && st.Note.ToLower().Contains(keyword))
                    );
                }

                // Order by Id descending
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

                return new ApiResponse<PaginatedResponse<SubjectTypeResponse>>(0, "Success", paginatedResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedResponse<SubjectTypeResponse>>(1, $"Error getting subject types: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<SubjectTypeResponse>> GetSubjectTypeByIdAsync(int id)
        {
            var subjectType = await _context.SubjectTypes
                .FirstOrDefaultAsync(st => st.Id == id && !(st.IsDelete ?? false));

            if (subjectType == null)
                return new ApiResponse<SubjectTypeResponse>(1, "SubjectType not found", null);

            var response = _mapper.Map<SubjectTypeResponse>(subjectType);
            return new ApiResponse<SubjectTypeResponse>(0, "Success", response);
        }

        public async Task<ApiResponse<SubjectTypeResponse>> CreateSubjectTypeAsync(SubjectTypeRequest request)
        {
            var subjectType = _mapper.Map<SubjectType>(request);
            subjectType.CreateAt = DateTime.UtcNow.ToLocalTime();
            subjectType.IsDelete = false;

            await _context.SubjectTypes.AddAsync(subjectType);
            await _context.SaveChangesAsync();

            var response = _mapper.Map<SubjectTypeResponse>(subjectType);
            return new ApiResponse<SubjectTypeResponse>(0, "SubjectType created successfully", response);
        }

        public async Task<ApiResponse<SubjectTypeResponse>> UpdateSubjectTypeAsync(SubjectTypeRequest request)
        {
            var existingSubjectType = await _context.SubjectTypes
                .FirstOrDefaultAsync(st => st.Id == request.Id && !(st.IsDelete ?? false));

            if (existingSubjectType == null)
                return new ApiResponse<SubjectTypeResponse>(1, "SubjectType not found", null);

            _mapper.Map(request, existingSubjectType);
            existingSubjectType.UpdateAt = DateTime.UtcNow.ToLocalTime();

            await _context.SaveChangesAsync();

            var response = _mapper.Map<SubjectTypeResponse>(existingSubjectType);
            return new ApiResponse<SubjectTypeResponse>(0, "SubjectType updated successfully", response);
        }

        public async Task<ApiResponse<bool>> DeleteSubjectTypeAsync(List<int> ids)
        {
            try
            {
                var subjectTypes = await _context.SubjectTypes
                    .Where(st => ids.Contains(st.Id) && (!st.IsDelete.HasValue || !st.IsDelete.Value))
                    .ToListAsync();

                if (!subjectTypes.Any())
                {
                    return new ApiResponse<bool>(1, "No subject types found to delete", false);
                }

                foreach (var subjectType in subjectTypes)
                {
                    subjectType.IsDelete = true;
                    subjectType.UpdateAt = DateTime.UtcNow.ToLocalTime();
                }

                await _context.SaveChangesAsync();
                return new ApiResponse<bool>(0, $"Successfully deleted {subjectTypes.Count} subject types", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(1, $"Error deleting subject types: {ex.Message}", false);
            }
        }
    }
}