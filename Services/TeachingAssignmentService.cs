﻿using Microsoft.EntityFrameworkCore;
using Namespace.DTOs.Response.TopicListResponseDto;
using NuGet.Protocol;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services;

public class TeachingAssignmentService : ITeachingAssignmentService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;
    private readonly ITopicRepository _topicRepository;
    private readonly ITeachingAssignmentRepository _teachingAssignmentRepository;   
    public TeachingAssignmentService(ApplicationDbContext context, IAuthService authService, ITopicRepository topicRepository, ITeachingAssignmentRepository teachingAssignmentRepository)

    {
        _context = context;
        _authService = authService;
        _topicRepository = topicRepository;
        _teachingAssignmentRepository = teachingAssignmentRepository;
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
            FullName = assignment?.User?.FullName,
            ClassId = assignment?.ClassId,
            ClassName = assignment?.Class?.Name,
            SubjectId = assignment?.SubjectId,
            SubjectName = assignment?.Subject?.SubjectName,
            StartDate = assignment?.StartDate,
            EndDate = assignment?.EndDate,
            Description = assignment?.Description
        };
    }
    public async Task<TeachingAssignmentResponseCreateUpdate> Create(TeachingAssignmentRequestCreate request)
    {
        try
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                throw new UnauthorizedAccessException("Token không hợp lệ hoặc đã hết hạn!");

            // Kiểm tra trạng thái Active của người dùng được phân công
            var assignedUser = await _context.Users
            .Where(u => u.Id == request.UserId && (u.IsDelete == false || u.IsDelete == null))
            .Select(u => new
            {
                u.FullName,
                u.TeacherStatusId,
                StatusName = u.TeacherStatus != null ? u.TeacherStatus.StatusName : null,
                u.RoleId,
                RoleName = u.Role != null ? u.Role.Name : null
            })
            .FirstOrDefaultAsync();

            if (assignedUser == null)
            {
                throw new BadRequestException($"Không tìm thấy người dùng với ID {request.UserId}.");
            }

            if (assignedUser.RoleId != 2)
            {
                throw new BadRequestException($"Người dùng {assignedUser.FullName} không phải là giảng viên (hiện tại: {assignedUser.RoleName ?? "Không xác định"}).");
            }

            if (assignedUser.TeacherStatusId != 1)
            {
                throw new BadRequestException($"Giảng viên {assignedUser.FullName} không ở trạng thái giảng viên đang làm việc (hiện tại: {assignedUser.StatusName ?? "Không xác định"}).");
            }

            // Kiểm tra StartDate và EndDate: EndDate phải lớn hơn hoặc bằng StartDate
            if (request.EndDate < request.StartDate)
            {
                throw new BadRequestException($"Ngày kết thúc ({request.EndDate}) phải lớn hơn hoặc bằng ngày bắt đầu ({request.StartDate}).");
            }

            // Kiểm tra niên khóa và học kỳ hiện tại
            var now = DateTime.UtcNow;
            var currentSemester = await _context.Semesters
                .Include(s => s.AcademicYear)
                .Where(s => s.IsDelete != true
                         && s.StartDate.HasValue && s.EndDate.HasValue
                         && s.StartDate.Value <= now
                         && s.EndDate.Value >= now)
                .FirstOrDefaultAsync();

            Semester semesterToCheck = null!;

            if (currentSemester != null)
            {
                // Có học kỳ hiện tại: Kiểm tra StartDate và EndDate
                if (currentSemester.StartDate.HasValue && currentSemester.EndDate.HasValue &&
                    (request.StartDate < currentSemester.StartDate.Value || request.StartDate > currentSemester.EndDate.Value))
                {
                    throw new BadRequestException($"Ngày bắt đầu ({request.StartDate}) phải nằm trong học kỳ hiện tại ({currentSemester.Name}: {currentSemester.StartDate.Value} - {currentSemester.EndDate.Value}).");
                }

                if (currentSemester.StartDate.HasValue && currentSemester.EndDate.HasValue &&
                    (request.EndDate < currentSemester.StartDate.Value || request.EndDate > currentSemester.EndDate.Value))
                {
                    throw new BadRequestException($"Ngày kết thúc ({request.EndDate}) phải nằm trong học kỳ hiện tại ({currentSemester.Name}: {currentSemester.StartDate.Value} - {currentSemester.EndDate.Value}).");
                }

                semesterToCheck = currentSemester;

                // Log thông tin học kỳ hiện tại
                Console.WriteLine($"Insert vào học kỳ hiện tại: {currentSemester.Name}, niên khóa: {currentSemester.AcademicYear?.StartDate?.Year} - {currentSemester.AcademicYear?.EndDate?.Year}");
            }
            else
            {
                // Không có học kỳ hiện tại (mùa hè): Tìm học kỳ tiếp theo
                var nextSemester = await _context.Semesters
                    .Include(s => s.AcademicYear)
                    .Where(s => s.IsDelete != true
                             && s.StartDate.HasValue
                             && s.StartDate.Value > now)
                    .OrderBy(s => s.StartDate)
                    .FirstOrDefaultAsync();

                if (nextSemester == null)
                {
                    throw new BadRequestException("Hiện tại không có học kỳ nào để insert. Vui lòng tạo niên khóa và học kỳ mới.");
                }

                // Kiểm tra StartDate và EndDate trong học kỳ tiếp theo
                if (nextSemester.StartDate.HasValue && nextSemester.EndDate.HasValue &&
                    (request.StartDate < nextSemester.StartDate.Value || request.StartDate > nextSemester.EndDate.Value))
                {
                    throw new BadRequestException($"Ngày bắt đầu ({request.StartDate}) phải nằm trong học kỳ tiếp theo ({nextSemester.Name}: {nextSemester.StartDate.Value} - {nextSemester.EndDate.Value}).");
                }

                if (nextSemester.StartDate.HasValue && nextSemester.EndDate.HasValue &&
                    (request.EndDate < nextSemester.StartDate.Value || request.EndDate > nextSemester.EndDate.Value))
                {
                    throw new BadRequestException($"Ngày kết thúc ({request.EndDate}) phải nằm trong học kỳ tiếp theo ({nextSemester.Name}: {nextSemester.StartDate.Value} - {nextSemester.EndDate.Value}).");
                }

                semesterToCheck = nextSemester;

                // Log thông tin học kỳ tiếp theo
                Console.WriteLine($"Insert vào học kỳ tiếp theo: {nextSemester.Name}, niên khóa: {nextSemester.AcademicYear?.StartDate?.Year} - {nextSemester.AcademicYear?.EndDate?.Year}");
            }


            // Kiểm tra xem giảng viên có được dạy môn học này không
            // Lấy thông tin môn học
            var subject = await _context.Subjects
                .Where(s => s.Id == request.SubjectId && (s.IsDelete == false || s.IsDelete == null))
                .Select(s => new { s.SubjectName })
                .FirstOrDefaultAsync();

            if (subject == null)
            {
                throw new BadRequestException($"Không tìm thấy môn học với ID {request.SubjectId}.");
            }

            // Lấy thông tin lớp học
            var classInfo = await _context.Classes
                .Where(c => c.Id == request.ClassId && (c.IsDelete == false || c.IsDelete == null))
                .Select(c => new { c.Name })
                .FirstOrDefaultAsync();

            if (classInfo == null)
            {
                throw new BadRequestException($"Không tìm thấy lớp học với ID {request.ClassId}.");
            }

            // Kiểm tra xem giảng viên có được dạy môn học này không
            var teacherSubjectExists = await _context.TeacherClassSubjects
                .AnyAsync(tcs => tcs.UserId == request.UserId
                              && tcs.SubjectsId == request.SubjectId
                              && (tcs.IsDelete == false || tcs.IsDelete == null));

            if (!teacherSubjectExists)
            {
                throw new BadRequestException($"Giảng viên {assignedUser.FullName} không được phép dạy môn {subject.SubjectName} ở lớp {classInfo.Name}.");
            }
            // Kiểm tra xem lớp có được phép dạy môn học này không (dựa vào bảng ClassSubject)
            var classSubjectExists = await _context.ClassSubjects
                .AnyAsync(cs => cs.ClassId == request.ClassId
                             && cs.SubjectId == request.SubjectId
                             && (cs.IsDelete == false || cs.IsDelete == null));

            if (!classSubjectExists)
            {
                throw new BadRequestException($"Lớp {classInfo.Name} không được phép dạy môn học {subject.SubjectName}.");
            }

            // Kiểm tra xem giảng viên đã được phân công dạy môn này, ở lớp này, trong học kỳ này chưa
            var existingAssignment = await _context.TeachingAssignments
                .Where(ta => ta.UserId == request.UserId
                          && ta.SubjectId == request.SubjectId
                          && ta.ClassId == request.ClassId
                          && (ta.IsDelete == false || ta.IsDelete == null)
                          && ta.StartDate >= semesterToCheck.StartDate
                          && ta.EndDate <= semesterToCheck.EndDate)
                .FirstOrDefaultAsync();

            if (existingAssignment != null)
            {
                throw new BadRequestException($"Giảng viên {assignedUser.FullName} đã được phân công dạy môn {subject.SubjectName} ở lớp {classInfo.Name} trong học kỳ {semesterToCheck.Name}.");
            }
            // Tạo TeachingAssignment
            var assignment = new TeachingAssignment
            {
                UserId = request.UserId,
                ClassId = request.ClassId,
                SubjectId = request.SubjectId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Description = request.Description,
                UserCreate = user.Id
            };

            _context.TeachingAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            Console.WriteLine($"TeachingAssignment đã tạo với ID = {assignment.Id}");

            var result = await GetById(assignment.Id);
            if (result == null)
            {
                throw new InvalidOperationException($"Không thể lấy thông tin phân công giảng dạy với ID {assignment.Id} sau khi tạo.");
            }
            return result;
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine($"Lỗi khi lưu vào database: {dbEx.InnerException?.Message}");
            throw;
        }
    }
    public async Task<TeachingAssignmentResponseCreateUpdate> UpdateById(TeachingAssignmentRequestUpdate request)
    {
        try
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                throw new UnauthorizedAccessException("Token không hợp lệ hoặc đã hết hạn!");

            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.EndDate < request.StartDate)
            {
                throw new BadRequestException($"EndDate ({request.EndDate}) phải lớn hơn hoặc bằng StartDate ({request.StartDate}).");
            }

            // Lấy thông tin TeachingAssignment hiện tại
            var assignment = await _context.TeachingAssignments
                .Include(ta => ta.Subject)
                .Include(ta => ta.Class)
                .Include(ta => ta.User)
                .FirstOrDefaultAsync(ta => ta.Id == request.teachingAssignmentId && ta.IsDelete != true);

            if (assignment == null)
                throw new NotFoundException("Không tìm thấy phân công giảng dạy này.");
            if (assignment.EndDate < DateTime.UtcNow)
            {
                throw new BadRequestException("Phân công giảng dạy đã kết thúc, không thể cập nhật.");
            }
            // Lấy UserId và SubjectId từ TeachingAssignment hiện tại
            var userId = assignment.UserId;
            var subjectId = assignment.SubjectId;

            // Kiểm tra lớp học mới có tồn tại không
            var classExists = await _context.Classes
                .AnyAsync(c => c.Id == request.ClassId && c.IsDelete != true);

            if (!classExists)
                throw new NotFoundException("Lớp học không tồn tại.");

            // Lấy thông tin môn học
            var subject = await _context.Subjects
                .Where(s => s.Id == subjectId && (s.IsDelete == false || s.IsDelete == null))
                .Select(s => new { s.SubjectName })
                .FirstOrDefaultAsync();

            if (subject == null)
            {
                throw new BadRequestException($"Không tìm thấy môn học với ID {subjectId}.");
            }

            // Lấy thông tin lớp học mới
            var classInfo = await _context.Classes
                .Where(c => c.Id == request.ClassId && (c.IsDelete == false || c.IsDelete == null))
                .Select(c => new { c.Name })
                .FirstOrDefaultAsync();

            if (classInfo == null)
            {
                throw new BadRequestException($"Không tìm thấy lớp học với ID {request.ClassId}.");
            }

            // Lấy thông tin giảng viên
            var assignedUser = await _context.Users
                .Where(u => u.Id == userId && (u.IsDelete == false || u.IsDelete == null))
                .Select(u => new
                {
                    u.FullName,
                    u.TeacherStatusId,
                    StatusName = u.TeacherStatus != null ? u.TeacherStatus.StatusName : null,
                    u.RoleId,
                    RoleName = u.Role != null ? u.Role.Name : null
                })
                .FirstOrDefaultAsync();

            if (assignedUser == null)
            {
                throw new BadRequestException($"Không tìm thấy người dùng với ID {userId}.");
            }

            // Kiểm tra xem giảng viên có được phép dạy môn học này không
            var teacherSubjectExists = await _context.TeacherClassSubjects
                .AnyAsync(tcs => tcs.UserId == userId
                              && tcs.SubjectsId == subjectId
                              && (tcs.IsDelete == false || tcs.IsDelete == null));

            if (!teacherSubjectExists)
            {
                throw new BadRequestException($"Giảng viên {assignedUser.FullName} không được phép dạy môn {subject.SubjectName}.");
            }

            // Kiểm tra xem lớp mới có được phép dạy môn học này không
            var classSubjectExists = await _context.ClassSubjects
                .AnyAsync(cs => cs.ClassId == request.ClassId
                             && cs.SubjectId == subjectId
                             && (cs.IsDelete == false || cs.IsDelete == null));

            if (!classSubjectExists)
            {
                throw new BadRequestException($"Lớp {classInfo.Name} không được phép dạy môn học {subject.SubjectName}.");
            }

            // Kiểm tra niên khóa và học kỳ
            var now = DateTime.UtcNow;

            // Tìm học kỳ ban đầu (dựa trên StartDate ban đầu)
            var originalSemester = await _context.Semesters
                .Include(s => s.AcademicYear)
                .Where(s => s.IsDelete != true
                         && s.StartDate.HasValue && s.EndDate.HasValue
                         && s.StartDate.Value <= assignment.StartDate
                         && s.EndDate.Value >= assignment.StartDate)
                .FirstOrDefaultAsync();

            if (originalSemester == null)
            {
                throw new BadRequestException("Không tìm thấy học kỳ cho StartDate ban đầu của TeachingAssignment.");
            }

            Console.WriteLine($"Học kỳ ban đầu: {originalSemester.Name}, niên khóa: {originalSemester.AcademicYear?.StartDate?.Year} - {originalSemester.AcademicYear?.EndDate?.Year}");

            // Kiểm tra thời điểm cập nhật: Nếu học kỳ đã kết thúc, không cho phép cập nhật
            if (originalSemester.EndDate.HasValue && now > originalSemester.EndDate.Value)
            {
                throw new BadRequestException($"Học kỳ ({originalSemester.Name}) đã kết thúc vào {originalSemester.EndDate.Value}. Không thể cập nhật TeachingAssignment sau khi học kỳ kết thúc.");
            }

            // Tìm học kỳ mới (dựa trên StartDate mới)
            var newSemester = await _context.Semesters
                .Include(s => s.AcademicYear)
                .Where(s => s.IsDelete != true
                         && s.StartDate.HasValue && s.EndDate.HasValue
                         && s.StartDate.Value <= request.StartDate
                         && s.EndDate.Value >= request.StartDate)
                .FirstOrDefaultAsync();

            if (newSemester == null)
            {
                throw new BadRequestException("Ngày bắt đầu mới không thuộc học kỳ nào.");
            }

            Console.WriteLine($"Học kỳ mới: {newSemester.Name}, niên khóa: {newSemester.AcademicYear?.StartDate?.Year} - {newSemester.AcademicYear?.EndDate?.Year}");

            // Kiểm tra xem giảng viên đã được phân công dạy môn này, ở lớp này, trong học kỳ mới chưa
            var existingAssignment = await _context.TeachingAssignments
                .Where(ta => ta.UserId == userId
                          && ta.SubjectId == subjectId
                          && ta.ClassId == request.ClassId
                          && ta.Id != request.teachingAssignmentId // Loại trừ bản ghi hiện tại
                          && (ta.IsDelete == false || ta.IsDelete == null)
                          && ta.StartDate >= newSemester.StartDate
                          && ta.EndDate <= newSemester.EndDate)
                .FirstOrDefaultAsync();

            if (existingAssignment != null)
            {
                throw new BadRequestException($"Giảng viên {assignedUser.FullName} đã được phân công dạy môn {subject.SubjectName} ở lớp {classInfo.Name} trong học kỳ {newSemester.Name}.");
            }

            // Tìm học kỳ hiện tại
            var currentSemester = await _context.Semesters
                .Include(s => s.AcademicYear)
                .Where(s => s.IsDelete != true
                         && s.StartDate.HasValue && s.EndDate.HasValue
                         && s.StartDate.Value <= now
                         && s.EndDate.Value >= now)
                .FirstOrDefaultAsync();

            if (currentSemester != null)
            {
                // Có học kỳ hiện tại
                Console.WriteLine($"Học kỳ hiện tại: {currentSemester.Name}, niên khóa: {currentSemester.AcademicYear?.StartDate?.Year} - {currentSemester.AcademicYear?.EndDate?.Year}");

                if (originalSemester.Id == newSemester.Id)
                {
                    // Học kỳ không thay đổi: Kiểm tra StartDate và EndDate
                    if (originalSemester.StartDate.HasValue && originalSemester.EndDate.HasValue &&
                        (request.StartDate < originalSemester.StartDate.Value || request.StartDate > originalSemester.EndDate.Value))
                    {
                        throw new BadRequestException($"Ngày bắt đầu ({request.StartDate}) phải nằm trong học kỳ ban đầu ({originalSemester.Name}: {originalSemester.StartDate.Value} - {originalSemester.EndDate.Value}).");
                    }

                    if (originalSemester.StartDate.HasValue && originalSemester.EndDate.HasValue &&
                        (request.EndDate < originalSemester.StartDate.Value || request.EndDate > originalSemester.EndDate.Value))
                    {
                        throw new BadRequestException($"Ngày kết thúc ({request.EndDate}) phải nằm trong học kỳ ban đầu ({originalSemester.Name}: {originalSemester.StartDate.Value} - {originalSemester.EndDate.Value}).");
                    }
                }
                else
                {
                    // Học kỳ thay đổi: Học kỳ mới phải là học kỳ hiện tại
                    if (newSemester.Id != currentSemester.Id)
                    {
                        throw new BadRequestException($"Học kỳ mới ({newSemester.Name}) không phải là học kỳ hiện tại ({currentSemester.Name}). Chỉ được phép cập nhật trong học kỳ hiện tại.");
                    }

                    // Kiểm tra StartDate và EndDate trong học kỳ hiện tại
                    if (currentSemester.StartDate.HasValue && currentSemester.EndDate.HasValue &&
                        (request.StartDate < currentSemester.StartDate.Value || request.StartDate > currentSemester.EndDate.Value))
                    {
                        throw new BadRequestException($"Ngày bắt đầu ({request.StartDate}) phải nằm trong học kỳ hiện tại ({currentSemester.Name}: {currentSemester.StartDate.Value} - {currentSemester.EndDate.Value}).");
                    }

                    if (currentSemester.StartDate.HasValue && currentSemester.EndDate.HasValue &&
                        (request.EndDate < currentSemester.StartDate.Value || request.EndDate > currentSemester.EndDate.Value))
                    {
                        throw new BadRequestException($"Ngày kết thúc ({request.EndDate}) phải nằm trong học kỳ hiện tại ({currentSemester.Name}: {currentSemester.StartDate.Value} - {currentSemester.EndDate.Value}).");
                    }
                }
            }
            else
            {
                // Không có học kỳ hiện tại (mùa hè)
                var nextSemester = await _context.Semesters
                    .Include(s => s.AcademicYear)
                    .Where(s => s.IsDelete != true
                             && s.StartDate.HasValue
                             && s.StartDate.Value > now)
                    .OrderBy(s => s.StartDate)
                    .FirstOrDefaultAsync();

                if (nextSemester == null)
                {
                    throw new BadRequestException("Hiện tại không có học kỳ nào để cập nhật. Vui lòng tạo niên khóa và học kỳ mới.");
                }

                Console.WriteLine($"Học kỳ tiếp theo: {nextSemester.Name}, niên khóa: {nextSemester.AcademicYear?.StartDate?.Year} - {nextSemester.AcademicYear?.EndDate?.Year}");

                if (originalSemester.Id == newSemester.Id)
                {
                    // Học kỳ không thay đổi: Kiểm tra StartDate và EndDate
                    if (originalSemester.StartDate.HasValue && originalSemester.EndDate.HasValue &&
                        (request.StartDate < originalSemester.StartDate.Value || request.StartDate > originalSemester.EndDate.Value))
                    {
                        throw new BadRequestException($"Ngày bắt đầu ({request.StartDate}) phải nằm trong học kỳ ban đầu ({originalSemester.Name}: {originalSemester.StartDate.Value} - {originalSemester.EndDate.Value}).");
                    }

                    if (originalSemester.StartDate.HasValue && originalSemester.EndDate.HasValue &&
                        (request.EndDate < originalSemester.StartDate.Value || request.EndDate > originalSemester.EndDate.Value))
                    {
                        throw new BadRequestException($"Ngày kết thúc ({request.EndDate}) phải nằm trong học kỳ ban đầu ({originalSemester.Name}: {originalSemester.StartDate.Value} - {originalSemester.EndDate.Value}).");
                    }
                }
                else
                {
                    // Học kỳ thay đổi: Học kỳ mới phải là học kỳ tiếp theo
                    if (newSemester.Id != nextSemester.Id)
                    {
                        throw new BadRequestException($"Học kỳ mới ({newSemester.Name}) không phải là học kỳ tiếp theo ({nextSemester.Name}). Trong mùa hè, chỉ được phép cập nhật trong học kỳ tiếp theo.");
                    }

                    // Kiểm tra StartDate và EndDate trong học kỳ tiếp theo
                    if (nextSemester.StartDate.HasValue && nextSemester.EndDate.HasValue &&
                        (request.StartDate < nextSemester.StartDate.Value || request.StartDate > nextSemester.EndDate.Value))
                    {
                        throw new BadRequestException($"Ngày bắt đầu ({request.StartDate}) phải nằm trong học kỳ tiếp theo ({nextSemester.Name}: {nextSemester.StartDate.Value} - {nextSemester.EndDate.Value}).");
                    }

                    if (nextSemester.StartDate.HasValue && nextSemester.EndDate.HasValue &&
                        (request.EndDate < nextSemester.StartDate.Value || request.EndDate > nextSemester.EndDate.Value))
                    {
                        throw new BadRequestException($"Ngày kết thúc ({request.EndDate}) phải nằm trong học kỳ tiếp theo ({nextSemester.Name}: {nextSemester.StartDate.Value} - {nextSemester.EndDate.Value}).");
                    }
                }
            }

            // Cập nhật thông tin
            assignment.ClassId = request.ClassId;
            assignment.StartDate = request.StartDate;
            assignment.EndDate = request.EndDate;
            assignment.UpdateAt = DateTime.UtcNow;
            assignment.Description = request.Description;
            assignment.UserUpdate = user.Id;

            await _context.SaveChangesAsync();
            Console.WriteLine($"TeachingAssignment đã cập nhật với ID = {assignment.Id}, Description = {assignment.Description}");

            return new TeachingAssignmentResponseCreateUpdate
            {
                Id = assignment.Id,
                UserId = assignment.UserId,
                FullName = assignment.User?.FullName,
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

    /// <summary>
    /// Lấy danh sách giáo viên và phân công giảng dạy dựa trên niên khóa, nhóm môn học, và giáo viên.
    /// </summary>
    /// <param name="academicYearId">ID của niên khóa (tùy chọn). Nếu có, lọc phân công giảng dạy theo StartDate và EndDate của niên khóa.</param>
    /// <param name="subjectGroupId">ID của nhóm môn học (tùy chọn).</param>
    /// <param name="userId">ID của giáo viên (tùy chọn). Nếu không truyền, trả về danh sách giáo viên và toàn bộ phân công giảng dạy.</param>
    /// <returns>Trả về danh sách giáo viên và phân công giảng dạy.</returns>
    /// <exception cref="NotFoundException">Ném ra khi userId hoặc academicYearId không hợp lệ.</exception>
    public async Task<PaginatedResponse<TeachingAssignmentResponseCreateUpdate>> GetTeachingAssignments(
        int? academicYearId, int? subjectGroupId, int? userId, int pageNumber = 1, int pageSize = 10)
    {
        // Bước 1: Kiểm tra userId (nếu có)
        if (userId.HasValue)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == userId.Value
                            && u.Role != null && u.Role.Id == 2
                            && (u.IsDelete == false || u.IsDelete == null));

            if (!userExists)
            {
                throw new NotFoundException($"Giáo viên với ID = {userId.Value} không tồn tại hoặc không phải là giáo viên.");
            }
        }

        // Bước 2: Lấy thông tin niên khóa (nếu có academicYearId)
        DateTime? academicYearStartDate = null;
        DateTime? academicYearEndDate = null;

        if (academicYearId.HasValue)
        {
            var academicYear = await _context.AcademicYears
                .Where(ay => ay.Id == academicYearId.Value && (ay.IsDelete == false || ay.IsDelete == null))
                .FirstOrDefaultAsync();

            if (academicYear == null)
            {
                throw new NotFoundException($"Không tìm thấy niên khóa với ID = {academicYearId.Value}.");
            }

            if (!academicYear.StartDate.HasValue || !academicYear.EndDate.HasValue)
            {
                throw new BadRequestException($"Niên khóa với ID {academicYear.Id} không có thông tin StartDate hoặc EndDate hợp lệ.");
            }

            academicYearStartDate = academicYear.StartDate.Value;
            academicYearEndDate = academicYear.EndDate.Value;

            Console.WriteLine($"Niên khóa: {academicYearStartDate?.Year} - {academicYearEndDate?.Year}, StartDate: {academicYearStartDate}, EndDate: {academicYearEndDate}");
        }

        // Bước 3: Lấy danh sách giáo viên
        var teachersQuery = _context.Users
            .Where(u => u.Role != null && u.Role.Id == 2
                     && (u.IsDelete == false || u.IsDelete == null));

        // Luôn lọc giáo viên dựa trên TeachingAssignments
        var assignmentsSubQuery = _context.TeachingAssignments
            .Where(t => (t.IsDelete == false || t.IsDelete == null));

        if (academicYearId.HasValue)
        {
            assignmentsSubQuery = assignmentsSubQuery
                .Where(t => t.StartDate >= academicYearStartDate
                         && t.EndDate <= academicYearEndDate);
        }

        if (subjectGroupId.HasValue)
        {
            assignmentsSubQuery = assignmentsSubQuery
                .Join(_context.SubjectGroupSubjects,
                      t => t.SubjectId,
                      sgs => sgs.SubjectId,
                      (t, sgs) => new { t, sgs })
                .Where(x => x.sgs.SubjectGroupId == subjectGroupId.Value
                         && (x.sgs.SubjectGroup != null && (x.sgs.SubjectGroup.IsDelete == false || x.sgs.SubjectGroup.IsDelete == null)))
                .Select(x => x.t);
        }

        // Bước 4: Lấy danh sách phân công giảng dạy
        IQueryable<TeachingAssignment> assignmentsQuery = _context.TeachingAssignments
            .Where(t => (t.IsDelete == false || t.IsDelete == null)
                     && (!userId.HasValue || t.UserId == userId.Value));

        if (academicYearId.HasValue)
        {
            assignmentsQuery = assignmentsQuery
                .Where(t => t.StartDate >= academicYearStartDate
                         && t.EndDate <= academicYearEndDate);
        }

        if (subjectGroupId.HasValue)
        {
            assignmentsQuery = assignmentsQuery
                .Join(_context.SubjectGroupSubjects,
                      t => t.SubjectId,
                      sgs => sgs.SubjectId,
                      (t, sgs) => new { t, sgs })
                .Where(x => x.sgs.SubjectGroupId == subjectGroupId.Value
                         && (x.sgs.SubjectGroup != null && (x.sgs.SubjectGroup.IsDelete == false || x.sgs.SubjectGroup.IsDelete == null)))
                .Select(x => x.t);
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
                ClassCode = t.Class != null ? t.Class.ClassCode : null,
                UserId = t.UserId,
                FullName = t.User != null ? t.User.FullName : null,
                ClassId = t.ClassId,
                ClassName = t.Class != null ? t.Class.Name : null,
                SubjectId = t.SubjectId,
                SubjectName = t.Subject != null ? t.Subject.SubjectName : null,
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

        return assignments;
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

    public async Task<ApiResponse<TopicListResponseDto>> GetTopicsByAssignmentIdPaginatedAsync(int teachingAssignmentId, PaginationRequest request)
    {
        try
        {
            // 1. Lấy thông tin về teachingAssignment
            var teachingAssignment = await _teachingAssignmentRepository.GetByIdWithDetailsAsync(teachingAssignmentId);

            if (teachingAssignment == null)
            {
                return new ApiResponse<TopicListResponseDto>(1, "Không tìm thấy thông tin phân công giảng dạy");
            }

            // 2. Lấy tổng số bản ghi và dữ liệu phân trang
            var totalItems = await _topicRepository.CountByAssignmentIdAsync(teachingAssignmentId);

            if (totalItems == 0)
            {
                // Vẫn tạo response với danh sách trống
                var emptyResponse = new TopicListResponseDto
                {
                    TeacherName = teachingAssignment.User?.FullName,
                    ClassName = teachingAssignment.Class?.Name,
                    SubjectName = teachingAssignment.Subject?.SubjectName,
                    TopicItems = new PaginatedResponse<TopicItemDto>
                    {
                        Items = new List<TopicItemDto>(),
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalItems = 0,
                        TotalPages = 0,
                        HasNextPage = false,
                        HasPreviousPage = false
                    }
                };

                return new ApiResponse<TopicListResponseDto>(0, "Không có dữ liệu chủ đề", emptyResponse);
            }

            // 3. Lấy danh sách topics theo phân trang
            var topics = await _topicRepository.GetByAssignmentIdPaginatedAsync(
                teachingAssignmentId,
                request.PageNumber,
                request.PageSize);

            // 4. Chuyển đổi topics thành TopicItemDto
            var topicItems = topics.Select(t => new TopicItemDto
            {
                TopicId = t.Id,
                Title = t.Title,
                Description = t.Description,
                CloseAt = t.CloseAt,
            }).ToList();

            // 5. Tính toán thông tin phân trang
            int totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

            // 6. Tạo PaginatedResponse cho TopicItems
            var paginatedTopics = new PaginatedResponse<TopicItemDto>
            {
                Items = topicItems,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = request.PageNumber > 1,
                HasNextPage = request.PageNumber < totalPages
            };

            // 7. Tạo và trả về kết quả cuối cùng
            var result = new TopicListResponseDto
            {
                TeacherName = teachingAssignment.User?.FullName,
                ClassName = teachingAssignment.Class?.Name,
                SubjectName = teachingAssignment.Subject?.SubjectName,
                TopicItems = paginatedTopics
            };

            return new ApiResponse<TopicListResponseDto>(0, "Lấy dữ liệu thành công!", result);
        }
        catch (Exception ex)
        {
            return new ApiResponse<TopicListResponseDto>(1, "Đã xảy ra lỗi khi lấy dữ liệu chủ đề");
        }
    }



}