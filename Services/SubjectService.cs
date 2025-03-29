using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public SubjectService(ApplicationDbContext context, IMapper mapper, IAuthService authService)
        {
            _context = context;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<ApiResponse<PaginatedResponse<SubjectResponse>>> GetAllSubjectsAsync(string? keyword, int pageNumber, int pageSize)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<PaginatedResponse<SubjectResponse>>(1, "Không có quyền truy cập", null);

                var query = _context.Subjects
                    .Where(s => !s.IsDelete.HasValue || !s.IsDelete.Value);

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    keyword = keyword.Trim().ToLower();
                    query = query.Where(s =>
                        (s.SubjectCode != null && s.SubjectCode.ToLower().Contains(keyword)) ||
                        (s.SubjectName != null && s.SubjectName.ToLower().Contains(keyword))
                    );
                }

                query = query
                    .Include(s => s.SubjectType)
                    .OrderByDescending(s => s.Id);

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

                return new ApiResponse<PaginatedResponse<SubjectResponse>>(0, "Lấy danh sách môn học thành công", paginatedResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedResponse<SubjectResponse>>(1, $"Lỗi khi lấy danh sách môn học: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<SubjectResponse>> GetSubjectByIdAsync(int id)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<SubjectResponse>(1, "Không có quyền truy cập", null);

                var subject = await _context.Subjects
                    .Include(s => s.SubjectType)
                    .FirstOrDefaultAsync(s => s.Id == id && (!s.IsDelete.HasValue || !s.IsDelete.Value));

                if (subject == null)
                    return new ApiResponse<SubjectResponse>(1, "Không tìm thấy môn học", null);

                var response = _mapper.Map<SubjectResponse>(subject);
                return new ApiResponse<SubjectResponse>(0, "Lấy thông tin môn học thành công", response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<SubjectResponse>(1, $"Lỗi khi lấy thông tin môn học: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<SubjectResponse>> CreateSubjectAsync(SubjectRequest request)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<SubjectResponse>(1, "Không có quyền truy cập", null);

                if (request == null)
                    return new ApiResponse<SubjectResponse>(1, "Dữ liệu không hợp lệ", null);

                if (string.IsNullOrEmpty(request.SubjectCode))
                    return new ApiResponse<SubjectResponse>(1, "Mã môn học không được để trống", null);

                if (await _context.Subjects.AnyAsync(s => s.SubjectCode == request.SubjectCode && (!s.IsDelete.HasValue || !s.IsDelete.Value)))
                    return new ApiResponse<SubjectResponse>(1, "Mã môn học đã tồn tại", null);

                var subjectType = await _context.SubjectTypes.FindAsync(request.SubjectTypeId);
                if (subjectType == null)
                    return new ApiResponse<SubjectResponse>(1, "Không tìm thấy loại môn học", null);

                var subject = _mapper.Map<Subject>(request);
                subject.CreateAt = DateTime.UtcNow.ToLocalTime();
                subject.IsDelete = false;
                subject.UserCreate = user.Id;

                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();

                await _context.Entry(subject)
                    .Reference(s => s.SubjectType)
                    .LoadAsync();

                var response = _mapper.Map<SubjectResponse>(subject);
                return new ApiResponse<SubjectResponse>(0, "Tạo môn học thành công", response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<SubjectResponse>(1, $"Lỗi khi tạo môn học: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<SubjectResponse>> UpdateSubjectAsync(SubjectRequest request)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<SubjectResponse>(1, "Không có quyền truy cập", null);

                if (request == null)
                    return new ApiResponse<SubjectResponse>(1, "Dữ liệu không hợp lệ", null);

                if (string.IsNullOrEmpty(request.SubjectCode))
                    return new ApiResponse<SubjectResponse>(1, "Mã môn học không được để trống", null);

                var existingSubject = await _context.Subjects
                    .Include(s => s.SubjectType)
                    .FirstOrDefaultAsync(s => s.Id == request.Id && (!s.IsDelete.HasValue || !s.IsDelete.Value));

                if (existingSubject == null)
                    return new ApiResponse<SubjectResponse>(1, "Không tìm thấy môn học", null);

                if (await _context.Subjects.AnyAsync(s =>
                    s.SubjectCode == request.SubjectCode &&
                    s.Id != request.Id &&
                    (!s.IsDelete.HasValue || !s.IsDelete.Value)))
                {
                    return new ApiResponse<SubjectResponse>(1, "Mã môn học đã tồn tại", null);
                }

                var subjectType = await _context.SubjectTypes.FindAsync(request.SubjectTypeId);
                if (subjectType == null)
                    return new ApiResponse<SubjectResponse>(1, "Không tìm thấy loại môn học", null);

                _mapper.Map(request, existingSubject);
                existingSubject.UpdateAt = DateTime.UtcNow.ToLocalTime();
                existingSubject.UserUpdate = user.Id;

                await _context.SaveChangesAsync();

                var response = _mapper.Map<SubjectResponse>(existingSubject);
                return new ApiResponse<SubjectResponse>(0, "Cập nhật môn học thành công", response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<SubjectResponse>(1, $"Lỗi khi cập nhật môn học: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<bool>> DeleteSubjectAsync(int id)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<bool>(1, "Không có quyền truy cập", false);

                var subject = await _context.Subjects.FindAsync(id);
                if (subject == null || subject.IsDelete == true)
                    return new ApiResponse<bool>(1, "Không tìm thấy môn học", false);

                subject.IsDelete = true;
                subject.UpdateAt = DateTime.UtcNow.ToLocalTime();
                subject.UserUpdate = user.Id;

                await _context.SaveChangesAsync();
                return new ApiResponse<bool>(0, "Xóa môn học thành công", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(1, $"Lỗi khi xóa môn học: {ex.Message}", false);
            }
        }

        public async Task<ApiResponse<bool>> DeleteMultipleSubjectsAsync(List<int> ids)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new ApiResponse<bool>(1, "Không có quyền truy cập", false);

                var subjects = await _context.Subjects
                    .Where(s => ids.Contains(s.Id) && (!s.IsDelete.HasValue || !s.IsDelete.Value))
                    .ToListAsync();

                if (!subjects.Any())
                    return new ApiResponse<bool>(1, "Không tìm thấy môn học để xóa", false);

                foreach (var subject in subjects)
                {
                    subject.IsDelete = true;
                    subject.UpdateAt = DateTime.UtcNow.ToLocalTime();
                    subject.UserUpdate = user.Id;
                }

                await _context.SaveChangesAsync();
                return new ApiResponse<bool>(0, $"Đã xóa thành công {subjects.Count} môn học", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(1, $"Lỗi khi xóa môn học: {ex.Message}", false);
            }
        }
        public async Task<List<SubjectResponseSearch>> getSubjectByUserId(int userId)
        {
            // Lấy danh sách SubjectsId từ TeacherClassSubject dựa trên userId
            var subjectIds = await _context.TeacherClassSubjects
                .Where(tcs => tcs.UserId == userId && (tcs.IsDelete == null || tcs.IsDelete == false))
                .Select(tcs => tcs.SubjectsId)
                .ToListAsync();

            // Nếu không tìm thấy môn học nào được phân công cho userId
            if (!subjectIds.Any())
            {
                return new List<SubjectResponseSearch>();
            }

            // Lấy danh sách môn học từ Subjects dựa trên subjectIds
            var query = _context.Subjects.AsQueryable();

            query = query.Where(x => subjectIds.Contains(x.Id) && (x.IsDelete == null || x.IsDelete == false));

            var subjects = await query
                .Select(x => new SubjectResponseSearch { Id = x.Id, SubjectName = x.SubjectName })
                .ToListAsync();

            return subjects;
        }

        public async Task<List<SubjectDropdownResponse>> GetSubjectsBySubjectGroupIdAsync(int subjectGroupId)
        {
            if (subjectGroupId <= 0)
            {
                return new List<SubjectDropdownResponse>();
            }

            var subjects = await _context.SubjectGroupSubjects
                .Where(sgs => sgs.SubjectGroupId == subjectGroupId &&
                             (sgs.IsDelete == null || sgs.IsDelete == false) &&
                             sgs.Subject != null)
                .Select(sgs => new SubjectDropdownResponse
                {
                    Id = sgs.Subject!.Id, 
                    Name = sgs.Subject!.SubjectName ?? string.Empty
                })
                .ToListAsync();

            return subjects;
        }

        public async Task<List<SubjectResponseSearch>> SearchSubjectByKeywordAsync(string? keyword)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return new List<SubjectResponseSearch>();

                var query = _context.Subjects.AsQueryable();
                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.ToLower().Trim();
                    query = query.Where(x =>
                        (x.SubjectName != null && x.SubjectName.ToLower().Contains(keyword)) ||
                        (x.SubjectCode != null && x.SubjectCode.ToLower().Contains(keyword)) &&
                        (x.IsDelete == null || x.IsDelete == false));
                }
                else
                {
                    query = query.Where(x => x.IsDelete == null || x.IsDelete == false);
                }

                return await query
                    .Select(x => new SubjectResponseSearch
                    {
                        Id = x.Id,
                        SubjectName = x.SubjectName
                    })
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<SubjectResponseSearch>();
            }
        }
    }
}