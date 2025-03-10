
// using System.Net.Mail;
// using System.Net.WebSockets;
// using Microsoft.EntityFrameworkCore;
// using Project_LMS.Data;
// using Project_LMS.DTOs.Request;
// using Project_LMS.DTOs.Response;
// using Project_LMS.Interfaces.Services;
// using Project_LMS.Models;


// namespace Project_LMS.Services;

// public class TestExamService : ITestExamService
// {

//     private readonly ApplicationDbContext _context;

//     public TestExamService(ApplicationDbContext context)
//     {
//         _context = context;
//     }
//     public async Task<ApiResponse<PaginatedResponse<TestExamResponse>>> GetAllTestExamsAsync(string? keyword, int pageNumber, int pageSize)
//     {
//         var query = _context.TestExams
//             .Where(te => !te.IsDelete.HasValue || !te.IsDelete.Value);

//         if (!string.IsNullOrWhiteSpace(keyword))
//         {
//             keyword = keyword.Trim().ToLower();
//             query = query.Where(te =>
//                 (te.Topic != null && te.Topic.ToLower().Contains(keyword)) ||
//                 (te.Description != null && te.Description.ToLower().Contains(keyword))
//             );
//         }

//         query = query.Include(te => te.Department).Include(te => te.TestExamType);

//         var totalItems = await query.CountAsync();
//         var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

//         var testExams = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

//         var testExamResponses = testExams.Select(te => new TestExamResponse
//         {
//             Id = te.Id,
//             DepartmentId = te.DepartmentId,
//             TestExamTypeId = te.TestExamTypeId,
//             Topic = te.Topic,
//             Form = te.Form,
//             Duration = te.Duration,
//             StartDate = te.StartDate,
//             EndDate = te.EndDate,
//             Description = te.Description,
//             Attachment = te.Attachment
//         }).ToList();

//         var paginatedResponse = new PaginatedResponse<TestExamResponse>
//         {
//             Items = testExamResponses,
//             PageNumber = pageNumber,
//             PageSize = pageSize,
//             TotalItems = totalItems,
//             TotalPages = totalPages,
//             HasPreviousPage = pageNumber > 1,
//             HasNextPage = pageNumber < totalPages
//         };

//         return new ApiResponse<PaginatedResponse<TestExamResponse>>(0, "Success", paginatedResponse);
//     }

//     public async Task<ApiResponse<TestExamResponse>> GetTestExamByIdAsync(int id)
//     {
//         var testExam = await _context.TestExams
//             .Include(te => te.Department)
//             .Include(te => te.TestExamType)
//             .FirstOrDefaultAsync(te => te.Id == id && (!te.IsDelete.HasValue || !te.IsDelete.Value));

//         if (testExam == null)
//             return new ApiResponse<TestExamResponse>(1, "TestExam not found", null);

//         var response = new TestExamResponse
//         {
//             Id = testExam.Id,
//             DepartmentId = testExam.DepartmentId,
//             TestExamTypeId = testExam.TestExamTypeId,
//             Topic = testExam.Topic,
//             Form = testExam.Form,
//             Duration = testExam.Duration,
//             StartDate = testExam.StartDate,
//             EndDate = testExam.EndDate,
//             Description = testExam.Description,
//             Attachment = testExam.Attachment
//         };
//         return new ApiResponse<TestExamResponse>(0, "Success", response);
//     }

//     public async Task<ApiResponse<TestExamResponse>> CreateTestExamAsync(TestExamRequest request)
//     {
//         try
//         {
//             var testExam = new TestExam
//             {
//                 DepartmentId = request.DepartmentId,
//                 TestExamTypeId = request.TestExamTypeId,
//                 Topic = request.Topic,
//                 Form = request.Form,
//                 Duration = request.Duration,
//                 StartDate = request.StartDate,
//                 EndDate = request.EndDate,
//                 Description = request.Description,
//                 Attachment = request.Attachment,
//                 CreateAt = DateTime.UtcNow.ToLocalTime(),
//                 IsDelete = false
//             };

//             _context.TestExams.Add(testExam);
//             await _context.SaveChangesAsync();

//             var response = new TestExamResponse
//             {
//                 Id = testExam.Id,
//                 DepartmentId = testExam.DepartmentId,
//                 TestExamTypeId = testExam.TestExamTypeId,
//                 Topic = testExam.Topic,
//                 Form = testExam.Form,
//                 Duration = testExam.Duration,
//                 StartDate = testExam.StartDate,
//                 EndDate = testExam.EndDate,
//                 Description = testExam.Description,
//                 Attachment = testExam.Attachment
//             };
//             return new ApiResponse<TestExamResponse>(0, "TestExam created successfully", response);
//         }
//         catch (Exception ex)
//         {
//             return new ApiResponse<TestExamResponse>(1, $"Error creating TestExam: {ex.Message}", null);
//         }
//     }

//     public async Task<ApiResponse<TestExamResponse>> UpdateTestExamAsync(int id, TestExamRequest request)
//     {
//         try
//         {
//             var testExam = await _context.TestExams
//     .FirstOrDefaultAsync(te => te.Id == id && (te.IsDelete == null || te.IsDelete == false));

//             if (testExam == null || testExam.IsDelete == true)
//                 return new ApiResponse<TestExamResponse>(1, "TestExam not found", null);

//             testExam.DepartmentId = request.DepartmentId;
//             testExam.TestExamTypeId = request.TestExamTypeId;
//             testExam.Topic = request.Topic;
//             testExam.Form = request.Form;
//             testExam.Duration = request.Duration;
//             testExam.StartDate = request.StartDate;
//             testExam.EndDate = request.EndDate;
//             testExam.Description = request.Description;
//             testExam.Attachment = request.Attachment;
//             testExam.UpdateAt = DateTime.UtcNow.ToLocalTime();

//             await _context.SaveChangesAsync();

//             var response = new TestExamResponse
//             {
//                 Id = testExam.Id,
//                 DepartmentId = testExam.DepartmentId,
//                 TestExamTypeId = testExam.TestExamTypeId,
//                 Topic = testExam.Topic,
//                 Form = testExam.Form,
//                 Duration = testExam.Duration,
//                 StartDate = testExam.StartDate,
//                 EndDate = testExam.EndDate,
//                 Description = testExam.Description,
//                 Attachment = testExam.Attachment
//             };
//             return new ApiResponse<TestExamResponse>(0, "TestExam updated successfully", response);
//         }
//         catch (Exception ex)
//         {
//             return new ApiResponse<TestExamResponse>(1, $"Error updating TestExam: {ex.Message}", null);
//         }
//     }
//     public async Task<ApiResponse<bool>> DeleteTestExamAsync(int id)
//     {
//         try
//         {
//             var testExam = await _context.TestExams.FindAsync(id);
//             if (testExam == null || testExam.IsDelete == true)
//                 return new ApiResponse<bool>(1, "TestExam not found", false);

//             testExam.IsDelete = true;
//             testExam.UpdateAt = DateTime.UtcNow.ToLocalTime();

//             await _context.SaveChangesAsync();
//             return new ApiResponse<bool>(0, "TestExam deleted successfully", true);
//         }
//         catch (Exception ex)
//         {
//             return new ApiResponse<bool>(1, $"Error deleting TestExam: {ex.Message}", false);
//         }
//     }
// }
