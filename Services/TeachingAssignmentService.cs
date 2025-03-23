using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services;

public class TeachingAssignmentService : ITeachingAssignmentService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;
    public TeachingAssignmentService(ApplicationDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<TeachingAssignmentResponseCreateUpdate?> GetById(int id)
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

        return new TeachingAssignmentResponseCreateUpdate
        {
            Id = assignment.Id,
            UserId = assignment.UserId,
            UserName = assignment.User.FullName,
            ClassId = assignment.ClassId,
            ClassName = assignment.Class.Name,
            SubjectId = assignment.SubjectId,
            SubjectName = assignment.Subject.SubjectName,
            StartDate = assignment.StartDate,
            EndDate = assignment.EndDate,
            Description = assignment.Description
        };
    }

    public async Task<TeachingAssignmentResponseCreateUpdate> Create(TeachingAssignmentRequestCreate request)
    {
        try
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                throw new UnauthorizedAccessException("Token không hợp lệ hoặc đã hết hạn!");
            var assignment = new TeachingAssignment
            {
                UserId = request.UserId,
                ClassId = request.ClassId,
                SubjectId = request.SubjectId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CreateAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                Description = request.Description,
                UserCreate = user.Id
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


    public async Task<TeachingAssignmentResponseCreateUpdate> UpdateById(int assignmentId,
    TeachingAssignmentRequestUpdate request)
    {
        try
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                throw new UnauthorizedAccessException("Token không hợp lệ hoặc đã hết hạn!");

            if (request == null) throw new ArgumentNullException(nameof(request));

            // Lấy đúng theo assignmentId chứ không lấy theo userId nữa
            var assignment = await _context.TeachingAssignments
                .Include(x => x.User)
                .Include(x => x.Class)
                .Include(x => x.Subject)
                .FirstOrDefaultAsync(x => x.Id == assignmentId && x.IsDelete != true);

            if (assignment == null)
                throw new Exception("Không tìm thấy phân công giảng dạy này.");

            // Kiểm tra ClassId tồn tại
            var classExists = await _context.Classes
                .AnyAsync(c => c.Id == request.ClassId && c.IsDelete != true);

            if (!classExists)
                throw new Exception("Lớp học không tồn tại.");

            // Cập nhật thông tin (không đụng vào UserId)
            assignment.ClassId = request.ClassId;
            assignment.StartDate = request.StartDate;
            assignment.EndDate = request.EndDate;
            assignment.UpdateAt = DateTime.UtcNow;
            assignment.Description = request.Description;
            assignment.UserUpdate = user.Id;

            await _context.SaveChangesAsync();

            return new TeachingAssignmentResponseCreateUpdate
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
                Description = assignment.Description
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
        var user = await _authService.GetUserAsync();
        if (user == null)
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc đã hết hạn!");

        var assignments = await _context.TeachingAssignments
            .Where(x => ids.Contains(x.Id) && x.IsDelete != true)
            .ToListAsync();

        if (assignments == null || !assignments.Any()) return false;

        foreach (var assignment in assignments)
        {
            assignment.IsDelete = true;
            assignment.UserUpdate = user.Id;
            assignment.UpdateAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }


    public async Task<TeachingAssignmentWrapperResponse> GetTeachingAssignments(int? academicYearId,
        int? subjectGroupId, int? userId, int pageNumber = 1, int pageSize = 10)
    {

        // Bước 1: Lọc danh sách giáo viên
        var teachersQuery = _context.Users
            .Where(u => u.Role.Id == 2 && (u.IsDelete == false || u.IsDelete == null));
// =======
//         IQueryable<User> teachersQuery;

//         // Nếu không chọn năm học và bộ môn => lấy toàn bộ giáo viên RoleId = 2

//         teachersQuery = _context.Users
//             .Where(u => u.Role.Id == 2 && (u.IsDelete == false || u.IsDelete == null));

//         if (userId.HasValue)
//         {
//             teachersQuery = teachersQuery.Where(u => u.Id == userId.Value);
//         }
// >>>>>>> a0c3643183c1888cc8509037b938fe40ec8251d3

        if (academicYearId.HasValue || subjectGroupId.HasValue)
        {
            teachersQuery = teachersQuery.Where(u =>
                _context.TeachingAssignments.Any(t =>
                    t.UserId == u.Id &&
                    (t.IsDelete == false || t.IsDelete == null) &&
                    (!academicYearId.HasValue || t.Class.AcademicYearId == academicYearId.Value) &&
                    (!subjectGroupId.HasValue || _context.SubjectGroupSubjects.Any(sgs =>
                        sgs.SubjectId == t.SubjectId &&
                        sgs.SubjectGroupId == subjectGroupId.Value &&
                        (sgs.SubjectGroup.IsDelete == false || sgs.SubjectGroup.IsDelete == null)
                    ))
                )
            );
        }

        // Lấy danh sách giáo viên sau khi lọc
        var teachers = await teachersQuery
            .Select(u => new UserResponseTeachingAssignment
            {
                Id = u.Id,
                FullName = u.FullName
            }).ToListAsync();

        // Nếu không truyền userId => chỉ trả danh sách giáo viên
        if (!userId.HasValue)
        {
            return new TeachingAssignmentWrapperResponse
            {
                Teachers = teachers,
                TeachingAssignments = null
            };
        }

        // Bước 2: Kiểm tra userId có nằm trong danh sách giáo viên không
        var isUserExist = teachers.Any(t => t.Id == userId.Value);
        if (!isUserExist)
        {
            return null ;
        }

        // Bước 3: Nếu có userId và đúng giáo viên => lấy danh sách phân công
        var assignmentsQuery = _context.TeachingAssignments
            .Where(t => (t.IsDelete == false || t.IsDelete == null) && t.UserId == userId.Value);

        if (academicYearId.HasValue)
        {
            assignmentsQuery = assignmentsQuery.Where(t => t.Class.AcademicYearId == academicYearId.Value);
        }

        if (subjectGroupId.HasValue)
        {
            assignmentsQuery = assignmentsQuery.Where(t =>
                _context.SubjectGroupSubjects.Any(sgs =>
                    sgs.SubjectId == t.SubjectId &&
                    sgs.SubjectGroupId == subjectGroupId.Value &&
                    (sgs.SubjectGroup.IsDelete == false || sgs.SubjectGroup.IsDelete == null)
                )
            );
        }

        var totalItems = await assignmentsQuery.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var assignmentList = await assignmentsQuery
            .OrderByDescending(t => t.CreateAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TeachingAssignmentResponseCreateUpdate
            {
                Id = t.Id,
                UserId = t.UserId,
                UserName = t.User.FullName,
                ClassId = t.ClassId,
                ClassName = t.Class.Name,
                SubjectId = t.SubjectId,
                SubjectName = t.Subject.SubjectName,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Description = t.Description
            })
            .ToListAsync();

        var assignments = new PaginatedResponse<TeachingAssignmentResponseCreateUpdate>
        {
            Items = assignmentList,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber < totalPages
        };

        return new TeachingAssignmentWrapperResponse
        {
            Teachers = teachers,
            TeachingAssignments = assignments
        };
    }



    public async Task<List<TopicResponseByAssignmentId>> GetTopicsByAssignmentIdAsync(int assignmentId)
    {
        var topics = await (from ta in _context.TeachingAssignments
                            join t in _context.Topics on ta.Id equals t.TeachingAssignmentId
                            join u in _context.Users on ta.UserId equals u.Id
                            join c in _context.Classes on ta.ClassId equals c.Id
                            join s in _context.Subjects on ta.SubjectId equals s.Id
                            where ta.Id == assignmentId
                            select new TopicResponseByAssignmentId
                            {
                                TopicId = t.Id,
                                TeachingAssignmentId = ta.Id,
                                UserId = u.Id,
                                FullName = u.FullName,
                                ClassName = c.Name,
                                SubjectName = s.SubjectName,
                                Title = t.Title,
                                Description = t.Description,
                                CloseAt = t.CloseAt
                            }).ToListAsync();

        return topics;
    }

    public async Task<List<ClassResponseSearch>> SearchClass(string? keyword)
    {
        var query = _context.Classes.AsQueryable();
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.Name.Contains(keyword) && (x.IsDelete == null || x.IsDelete == false));
        }
        else
        {
            query = query.Where(x => x.IsDelete == null || x.IsDelete == false);
        }

        var classes = await query
            .Select(x => new ClassResponseSearch { Id = x.Id, ClassName = x.Name })
            .ToListAsync();

        return classes;
    }

    public async Task<List<SubjectResponseSearch>> SearchSubject(string? keyword)
    {
        var query = _context.Subjects.AsQueryable();
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.SubjectName.Contains(keyword) && (x.IsDelete == null || x.IsDelete == false));
        }
        else
        {
            query = query.Where(x => x.IsDelete == null || x.IsDelete == false);
        }

        var subjects = await query
            .Select(x => new SubjectResponseSearch { Id = x.Id, SubjectName = x.SubjectName })
            .ToListAsync();

        return subjects;
    }


}