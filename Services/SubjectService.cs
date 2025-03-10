// using AutoMapper;
// using Microsoft.EntityFrameworkCore;
// using Project_LMS.Data;
// using Project_LMS.DTOs.Request;
// using Project_LMS.DTOs.Response;
// using Project_LMS.Interfaces.Services;
// using Project_LMS.Models;

// namespace Project_LMS.Services
// {
//     public class SubjectService : ISubjectService
//     {
//         private readonly ApplicationDbContext _context;
//         private readonly IMapper _mapper;

//         public SubjectService(ApplicationDbContext context, IMapper mapper)
//         {
//             _context = context;
//             _mapper = mapper;
//         }

//         public async Task<ApiResponse<PaginatedResponse<SubjectResponse>>> GetAllSubjectsAsync(string? keyword, int pageNumber, int pageSize)
//         {
//             try
//             {
//                 var query = _context.Subjects
//                     .Where(s => !s.IsDelete.HasValue || !s.IsDelete.Value);

//                 // Add search condition if keyword is provided
//                 if (!string.IsNullOrWhiteSpace(keyword))
//                 {
//                     keyword = keyword.Trim().ToLower();
//                     query = query.Where(s =>
//                         (s.SubjectCode != null && s.SubjectCode.ToLower().Contains(keyword)) ||
//                         (s.SubjectName != null && s.SubjectName.ToLower().Contains(keyword))
//                     );
//                 }

//                 // Include related entities
//                 query = query
//                     .Include(s => s.SubjectType)
//                     .Include(s => s.SubjectGroup)
//                     .OrderByDescending(s => s.Id);

//                 var totalItems = await query.CountAsync();
//                 var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

//                 var subjects = await query
//                     .Skip((pageNumber - 1) * pageSize)
//                     .Take(pageSize)
//                     .ToListAsync();

//                 var subjectResponses = _mapper.Map<List<SubjectResponse>>(subjects);

//                 var paginatedResponse = new PaginatedResponse<SubjectResponse>
//                 {
//                     Items = subjectResponses,
//                     PageNumber = pageNumber,
//                     PageSize = pageSize,
//                     TotalItems = totalItems,
//                     TotalPages = totalPages,
//                     HasPreviousPage = pageNumber > 1,
//                     HasNextPage = pageNumber < totalPages
//                 };

//                 return new ApiResponse<PaginatedResponse<SubjectResponse>>(0, "Success", paginatedResponse);
//             }
//             catch (Exception ex)
//             {
//                 return new ApiResponse<PaginatedResponse<SubjectResponse>>(1, $"Error getting subjects: {ex.Message}", null);
//             }
//         }

//         public async Task<ApiResponse<SubjectResponse>> GetSubjectByIdAsync(int id)
//         {
//             var subject = await _context.Subjects
//                 .Include(s => s.SubjectType)
//                 .Include(s => s.SubjectGroup)
//                 .FirstOrDefaultAsync(s => s.Id == id && (!s.IsDelete.HasValue || !s.IsDelete.Value));

//             if (subject == null)
//                 return new ApiResponse<SubjectResponse>(1, "Subject not found", null);

//             var response = _mapper.Map<SubjectResponse>(subject);
//             return new ApiResponse<SubjectResponse>(0, "Success", response);
//         }

//         public async Task<ApiResponse<SubjectResponse>> CreateSubjectAsync(SubjectRequest request)
//         {
//             try
//             {
//                 // Validate request
//                 if (request == null)
//                 {
//                     return new ApiResponse<SubjectResponse>(1, "Invalid request data", null);
//                 }

//                 if (string.IsNullOrEmpty(request.SubjectCode))
//                 {
//                     return new ApiResponse<SubjectResponse>(1, "Subject code is required", null);
//                 }

//                 // Check for duplicate SubjectCode
//                 if (await _context.Subjects
//                     .AnyAsync(s => s.SubjectCode == request.SubjectCode 
//                         && (!s.IsDelete.HasValue || !s.IsDelete.Value)))
//                 {
//                     return new ApiResponse<SubjectResponse>(1, "Subject code already exists", null);
//                 }

//                 // Validate foreign keys
//                 var subjectType = await _context.SubjectTypes.FindAsync(request.SubjectTypeId);
//                 if (subjectType == null)
//                 {
//                     return new ApiResponse<SubjectResponse>(1, "Subject type not found", null);
//                 }

//                 var subjectGroup = await _context.SubjectGroups.FindAsync(request.SubjectGroupId);
//                 if (subjectGroup == null)
//                 {
//                     return new ApiResponse<SubjectResponse>(1, "Subject group not found", null);
//                 }

//                 var subject = _mapper.Map<Subject>(request);
//                 subject.CreateAt = DateTime.UtcNow.ToLocalTime();
//                 subject.IsDelete = false;
//                 subject.Id = 0;

//                 _context.Subjects.Add(subject);
//                 await _context.SaveChangesAsync();

//                 await _context.Entry(subject)
//                     .Reference(s => s.SubjectType)
//                     .LoadAsync();
//                 await _context.Entry(subject)
//                     .Reference(s => s.SubjectGroup)
//                     .LoadAsync();

//                 var response = _mapper.Map<SubjectResponse>(subject);
//                 return new ApiResponse<SubjectResponse>(0, "Subject created successfully", response);
//             }
//             catch (Exception ex)
//             {
//                 return new ApiResponse<SubjectResponse>(1, $"Error creating subject: {ex.Message}", null);
//             }
//         }

//         public async Task<ApiResponse<SubjectResponse>> UpdateSubjectAsync(int id, SubjectRequest request)
//         {
//             try
//             {
//                 // Validate request
//                 if (request == null)
//                 {
//                     return new ApiResponse<SubjectResponse>(1, "Invalid request data", null);
//                 }

//                 if (string.IsNullOrEmpty(request.SubjectCode))
//                 {
//                     return new ApiResponse<SubjectResponse>(1, "Subject code is required", null);
//                 }

//                 // Check if subject exists
//                 var existingSubject = await _context.Subjects
//                     .Include(s => s.SubjectType)
//                     .Include(s => s.SubjectGroup)
//                     .FirstOrDefaultAsync(s => s.Id == id && (!s.IsDelete.HasValue || !s.IsDelete.Value));

//                 if (existingSubject == null)
//                     return new ApiResponse<SubjectResponse>(1, "Subject not found", null);

//                 // Check for duplicate SubjectCode, excluding current subject
//                 var duplicateExists = await _context.Subjects
//                     .AnyAsync(s => s.SubjectCode == request.SubjectCode 
//                         && s.Id != id 
//                         && (!s.IsDelete.HasValue || !s.IsDelete.Value));

//                 if (duplicateExists)
//                 {
//                     return new ApiResponse<SubjectResponse>(1, $"Subject code '{request.SubjectCode}' already exists", null);
//                 }

//                 // Validate foreign keys
//                 var subjectType = await _context.SubjectTypes.FindAsync(request.SubjectTypeId);
//                 if (subjectType == null)
//                 {
//                     return new ApiResponse<SubjectResponse>(1, "Subject type not found", null);
//                 }

//                 var subjectGroup = await _context.SubjectGroups.FindAsync(request.SubjectGroupId);
//                 if (subjectGroup == null)
//                 {
//                     return new ApiResponse<SubjectResponse>(1, "Subject group not found", null);
//                 }

//                 // Update subject
//                 _mapper.Map(request, existingSubject);
//                 existingSubject.UpdateAt = DateTime.UtcNow.ToLocalTime();

//                 await _context.SaveChangesAsync();

//                 var response = _mapper.Map<SubjectResponse>(existingSubject);
//                 return new ApiResponse<SubjectResponse>(0, "Subject updated successfully", response);
//             }
//             catch (Exception ex)
//             {
//                 return new ApiResponse<SubjectResponse>(1, $"Error updating subject: {ex.Message}", null);
//             }
//         }

//         public async Task<ApiResponse<bool>> DeleteSubjectAsync(int id)
//         {
//             try
//             {
//                 var subject = await _context.Subjects.FindAsync(id);
//                 if (subject == null || subject.IsDelete == true)
//                     return new ApiResponse<bool>(1, "Subject not found", false);

//                 subject.IsDelete = true;
//                 // Convert UTC to local time for PostgreSQL timestamp without time zone
//                 subject.UpdateAt = DateTime.UtcNow.ToLocalTime();

//                 await _context.SaveChangesAsync();
//                 return new ApiResponse<bool>(0, "Subject deleted successfully", true);
//             }
//             catch (Exception ex)
//             {
//                 return new ApiResponse<bool>(1, $"Error deleting subject: {ex.Message}", false);
//             }
//         }

//         public async Task<ApiResponse<bool>> DeleteMultipleSubjectsAsync(List<int> ids)
//         {
//             try
//             {
//                 var subjects = await _context.Subjects
//                     .Where(s => ids.Contains(s.Id) && (!s.IsDelete.HasValue || !s.IsDelete.Value))
//                     .ToListAsync();

//                 if (!subjects.Any())
//                 {
//                     return new ApiResponse<bool>(1, "No subjects found to delete", false);
//                 }

//                 foreach (var subject in subjects)
//                 {
//                     subject.IsDelete = true;
//                     subject.UpdateAt = DateTime.UtcNow.ToLocalTime();
//                 }

//                 await _context.SaveChangesAsync();
//                 return new ApiResponse<bool>(0, $"Successfully deleted {subjects.Count} subjects", true);
//             }
//             catch (Exception ex)
//             {
//                 return new ApiResponse<bool>(1, $"Error deleting subjects: {ex.Message}", false);
//             }
//         }
//     }
// }