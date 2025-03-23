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

        public ClassTypeService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PaginatedResponse<ClassTypeResponse>>> GetAllClassTypesAsync(string? keyword, int pageNumber, int pageSize)
        {
            try
            {
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

                return new ApiResponse<PaginatedResponse<ClassTypeResponse>>(0, "Success", paginatedResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedResponse<ClassTypeResponse>>(1, $"Error getting class types: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<ClassTypeResponse>> GetClassTypeByIdAsync(int id)
        {
            var classType = await _context.ClassTypes
                .FirstOrDefaultAsync(ct => ct.Id == id && !(ct.IsDelete ?? false));

            if (classType == null)
                return new ApiResponse<ClassTypeResponse>(1, "ClassType not found", null);

            var response = _mapper.Map<ClassTypeResponse>(classType);
            return new ApiResponse<ClassTypeResponse>(0, "Success", response);
        }

        public async Task<ApiResponse<ClassTypeResponse>> CreateClassTypeAsync(ClassTypeRequest request)
        {
            var classType = _mapper.Map<ClassType>(request);
            classType.CreateAt = DateTime.UtcNow.ToLocalTime();
            classType.IsDelete = false;

            await _context.ClassTypes.AddAsync(classType);
            await _context.SaveChangesAsync();

            var response = _mapper.Map<ClassTypeResponse>(classType);
            return new ApiResponse<ClassTypeResponse>(0, "ClassType created successfully", response);
        }

        public async Task<ApiResponse<ClassTypeResponse>> UpdateClassTypeAsync(ClassTypeRequest request)
        {
            var existingClassType = await _context.ClassTypes
                .FirstOrDefaultAsync(ct => ct.Id == request.id && !(ct.IsDelete ?? false));

            if (existingClassType == null)
                return new ApiResponse<ClassTypeResponse>(1, "ClassType not found", null);

            _mapper.Map(request, existingClassType);
            existingClassType.UpdateAt = DateTime.UtcNow.ToLocalTime();

            await _context.SaveChangesAsync();

            var response = _mapper.Map<ClassTypeResponse>(existingClassType);
            return new ApiResponse<ClassTypeResponse>(0, "ClassType updated successfully", response);
        }

        public async Task<ApiResponse<bool>> DeleteClassTypeAsync(List<int> ids)
        {
            try
            {
                var classTypes = await _context.ClassTypes
                    .Where(c => ids.Contains(c.Id) && (!c.IsDelete.HasValue || !c.IsDelete.Value))
                    .ToListAsync();

                if (!classTypes.Any())
                {
                    return new ApiResponse<bool>(1, "No class types found to delete", false);
                }

                foreach (var classType in classTypes)
                {
                    classType.IsDelete = true;
                    classType.UpdateAt = DateTime.UtcNow.ToLocalTime();
                }

                await _context.SaveChangesAsync();
                return new ApiResponse<bool>(0, $"Successfully deleted {classTypes.Count} class types", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(1, $"Error deleting class types: {ex.Message}", false);
            }
        }
    }
}
