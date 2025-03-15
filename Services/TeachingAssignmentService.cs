using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services;

public class TeachingAssignmentService : ITeachingAssignmentService
{
    private readonly ApplicationDbContext _context;

    public TeachingAssignmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<TeachingAssignmentResponse>> GetAll(int pageNumber, int pageSize)
    {
        var query = _context.TeachingAssignments
            .Where(t => t.IsDelete == false || t.IsDelete == null)
            .Include(t => t.User)
            .Include(t => t.Class)
            .Include(t => t.Subject);

        int totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var items = await query
            .OrderByDescending(t => t.CreateAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TeachingAssignmentResponse
            {
                Id = t.Id,
                UserId = t.UserId,
                UserName = t.User != null ? t.User.FullName : null,
                ClassId = t.ClassId,
                ClassName = t.Class != null ? t.Class.Name : null,
                SubjectId = t.SubjectId,
                SubjectName = t.Subject != null ? t.Subject.SubjectName : null,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                CreateAt = t.CreateAt
            })
            .ToListAsync();

        return new PaginatedResponse<TeachingAssignmentResponse>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber < totalPages
        };
    }

    public async Task<TeachingAssignmentResponse?> GetById(int id)
    {
        var assignment = await _context.TeachingAssignments
            .Include(t => t.User)
            .Include(t => t.Class)
            .Include(t => t.Subject)
            .FirstOrDefaultAsync(t => t.Id == id && (t.IsDelete == false || t.IsDelete == null));

        if (assignment == null)
        {
            Console.WriteLine($"Không tìm thấy TeachingAssignment với ID: {id}");
            return null;
        }

        return new TeachingAssignmentResponse
        {
            Id = assignment.Id,
            UserId = assignment.UserId,
            UserName = assignment.User?.FullName,
            ClassId = assignment.ClassId,
            ClassName = assignment.Class?.Name,
            SubjectId = assignment.SubjectId,
            SubjectName = assignment.Subject?.SubjectName,
            StartDate = assignment.StartDate,
            EndDate = assignment.EndDate,
            CreateAt = assignment.CreateAt
        };
    }


    public async Task<TeachingAssignmentResponse?> Create(TeachingAssignmentRequest request)
    {
        try
        {
            var assignment = new TeachingAssignment
            {
                UserId = request.UserId,
                ClassId = request.ClassId,
                SubjectId = request.SubjectId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CreateAt = DateTime.UtcNow
            };

            _context.TeachingAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            return await GetById(assignment.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi Create: {ex.Message} | {ex.StackTrace}");
            throw;
        }
    }



    public async Task<bool> Update(int id, TeachingAssignmentRequest request)
    {
        try
        {
            var assignment = await _context.TeachingAssignments.FindAsync(id);
            if (assignment == null) return false;

            assignment.UserId = request.UserId;
            assignment.ClassId = request.ClassId;
            assignment.SubjectId = request.SubjectId;
            assignment.StartDate = request.StartDate;
            assignment.EndDate = request.EndDate;
            assignment.UpdateAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi Update: {ex.Message} | {ex.StackTrace}");
            throw;
        }
    }



    public async Task<bool> Delete(int id)
    {
        var assignment = await _context.TeachingAssignments.FindAsync(id);
        if (assignment == null) return false;

        assignment.IsDelete = true;
        await _context.SaveChangesAsync();
        return true;
    }
}