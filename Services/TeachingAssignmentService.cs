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

    public async Task<PaginatedResponse<TeachingAssignmentResponse>> GetAll(int pageNumber, int pageSize, int? academicYearId, int? subjectGroupId)
    {
        var queryBase = _context.TeachingAssignments
            .Where(t => t.IsDelete == false || t.IsDelete == null)
            .Include(t => t.User)
            .Include(t => t.Class)
                .ThenInclude(c => c.AcademicYear)
            .Include(t => t.Subject)
            .Include(x => x.Topics);

        var query = queryBase.AsQueryable();

        // Filter theo Niên khóa (nếu có)
        if (academicYearId.HasValue)
        {
            query = query.Where(t => t.Class.AcademicYearId == academicYearId.Value);
        }

        // Filter theo Tổ bộ môn (nếu có)
        if (subjectGroupId.HasValue)
        {
            query = query.Where(t =>
                _context.SubjectGroupSubjects.Any(sgs =>
                    sgs.SubjectId == t.SubjectId &&
                    sgs.SubjectGroupId == subjectGroupId.Value &&
                    (sgs.SubjectGroup.IsDelete == false || sgs.SubjectGroup.IsDelete == null)
                )
            );
        }

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
                ClassId = t.ClassId,
                ClassName = t.Class.Name,
                SubjectId = t.SubjectId,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                CreateAt = t.CreateAt,
            //    Topics = t.Topics
            //.Where(topic => topic.IsDelete == false || topic.IsDelete == null)
            //.Select(topic => new TopicResponse
            //{
            //    Id = topic.Id,
            //    Title = topic.Title,
            //    FileName = topic.FileName,
            //    Description = topic.Description,
            //    CreateAt = topic.CreateAt
            //})
            //.ToList()
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
            ClassId = assignment.ClassId,
            SubjectId = assignment.SubjectId,
            StartDate = assignment.StartDate,
            EndDate = assignment.EndDate,
            CreateAt = assignment.CreateAt
        };
    }

    public async Task<List<TeachingAssignmentResponse>> GetByUserId(int userId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId && (u.IsDelete == false || u.IsDelete == null));
        if (!userExists)
        {
            throw new Exception("Không tìm thấy giảng viên");
        }

        var assignments = await _context.TeachingAssignments
            .Where(t => t.UserId == userId && (t.IsDelete == false || t.IsDelete == null))
            .Include(t => t.Class)
            .Include(t => t.Subject)
            .ToListAsync();

        if (!assignments.Any())
        {
            throw new Exception("Giảng viên chưa có phân công giảng dạy");
        }

        return assignments.Select(t => new TeachingAssignmentResponse
        {
            Id = t.Id,
            UserId = t.UserId,
            ClassId = t.ClassId,
            SubjectId = t.SubjectId,
            StartDate = t.StartDate,
            EndDate = t.EndDate,
            CreateAt = t.CreateAt
        }).ToList();
    }


    public async Task<TeachingAssignmentResponse> Create(TeachingAssignmentRequestCreate request)
    {
        try
        {
            //Console.WriteLine($"Bắt đầu tạo TeachingAssignment: UserId={request.UserId}, ClassId={request.ClassId}, SubjectId={request.SubjectId}");

            var assignment = new TeachingAssignment
            {
                UserId = request.UserId,
                ClassId = request.ClassId,
                SubjectId = request.SubjectId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CreateAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };

            _context.TeachingAssignments.Add(assignment);
            await _context.SaveChangesAsync(); 

            Console.WriteLine($"TeachingAssignment đã tạo với ID = {assignment.Id}");

            return await GetById(assignment.Id);
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine($"Lỗi khi lưu vào database: {dbEx.InnerException?.Message}");
            throw;
        }
    }


    public async Task<TeachingAssignmentResponse> UpdateByUserId(int userId, TeachingAssignmentRequest request)
    {
        try
        {
            // Kiểm tra request đầu vào
            if (request == null) throw new ArgumentNullException(nameof(request));

            // Tìm phân công theo userId (nếu 1 user chỉ có 1 phân công)
            var assignment = await _context.TeachingAssignments
                .FirstOrDefaultAsync(x => x.UserId == userId && x.IsDelete != true);

            if (assignment == null)
                throw new Exception("Không tìm thấy phân công giảng dạy cho giảng viên này.");

            // Cập nhật thông tin
            assignment.ClassId = request.ClassId;
            assignment.SubjectId = request.SubjectId;
            assignment.StartDate = request.StartDate;
            assignment.EndDate = request.EndDate;
            assignment.UpdateAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Trả về response
            return new TeachingAssignmentResponse
            {
                Id = assignment.Id,
                UserId = assignment.UserId,
                ClassId = assignment.ClassId,
                SubjectId = assignment.SubjectId,
                StartDate = assignment.StartDate,
                EndDate = assignment.EndDate,
                UpdateAt = assignment.UpdateAt,
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi Update: {ex.Message} | {ex.StackTrace}");
            throw;
        }
    }



    public async Task<bool> Delete(List<int> ids)
    {
        var assignments = await _context.TeachingAssignments
            .Where(x => ids.Contains(x.Id) && x.IsDelete != true)
            .ToListAsync();

        if (assignments == null || !assignments.Any()) return false;

        foreach (var assignment in assignments)
        {
            assignment.IsDelete = true;
        }

        await _context.SaveChangesAsync();
        return true;
    }

}