using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services;

public class WorkProcessService : IWorkProcessService
{
    private readonly ApplicationDbContext _context;

    public WorkProcessService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WorkProcessesResponse>> GetAllAsync(WorkProcessRequest request)
    {
        // Lấy thông tin AcademicYear nếu có
        var academicYear = request.AcademicYearId > 0
            ? await _context.AcademicYears.FindAsync(request.AcademicYearId)
            : null;

        // Lấy danh sách TeachingAssignments theo UserId
        var teachingAssignmentsQuery = _context.TeachingAssignments
            .Where(at => at.UserId == request.UserId);

        // Chỉ lọc theo ClassId nếu ClassId > 0
        if (request.ClassId > 0)
        {
            teachingAssignmentsQuery = teachingAssignmentsQuery.Where(at => at.ClassId == request.ClassId);
        }

        // Lấy danh sách TeachingAssignments (không gán null)
        var assignmentsTeaching = await teachingAssignmentsQuery.ToListAsync();

        // Truy vấn WorkProcesses
        var query = _context.WorkProcesses
            .Include(wp => wp.Department)
            .Include(wp => wp.Position)
            .Where(wp =>
                wp.IsDeleted == false &&
                wp.UserId == request.UserId);

        // Điều kiện AcademicYear
        if (academicYear != null)
        {
            query = query.Where(wp => wp.StartDate >= academicYear.StartDate && wp.EndDate <= academicYear.EndDate);
        }

        // Nếu có TeachingAssignments và ClassId > 0, sử dụng join để lọc
        if (assignmentsTeaching.Any() && request.ClassId > 0)
        {
            query = from wp in query
                    join ta in teachingAssignmentsQuery
                    on wp.UserId equals ta.UserId
                    where ta.StartDate <= wp.StartDate && ta.EndDate >= wp.EndDate
                    select wp;
        }

        // Thêm điều kiện tìm kiếm nếu có
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(wp =>
                wp.OrganizationUnit.Contains(request.Search) ||
                (wp.DepartmentId != null && wp.Department.Name.Contains(request.Search)) ||
                (wp.PositionId != null && wp.Position.Name.Contains(request.Search))
            );
        }

        // Trả về danh sách
        return await query.Select(wp => new WorkProcessesResponse
        {
            Id = wp.Id,
            OrganizationUnit = wp.OrganizationUnit,
            Department = wp.Department != null ? wp.Department.Name : null,
            Position = wp.Position != null ? wp.Position.Name : null,
            StartDate = wp.StartDate,
            EndDate = wp.EndDate
        }).ToListAsync();
    }



    public async Task<WorkProcessResponse> GetById(WorkProcessDeleteRequest request)
    {
        var workProcess = await _context.WorkProcesses
            .Include(wp => wp.Department)
            .Include(wp => wp.Position)
            .FirstOrDefaultAsync(wp => wp.Id == request.Id && wp.IsDeleted == false);

        if (workProcess == null) return null;
        return new WorkProcessResponse
        {
            Id = workProcess.Id,
            OrganizationUnit = workProcess.OrganizationUnit,
            DepartmentId = workProcess.DepartmentId,
            PositionId = workProcess.PositionId,
            StartDate = workProcess.StartDate,
            EndDate = workProcess.EndDate
        };
    }

    // Lấy tất cả WorkUnit không bao gồm các WorkUnit có Id trong chuỗi ids
    public async Task<IEnumerable<WorkUnitResponse>> GetWorkUnitExcluding(WorkUnitRequest request)
    {
        var query = _context.WorkUnits.AsQueryable(); // Khởi tạo query

        // Nếu request.Ids không null và có phần tử thì mới lọc
        if (request.Ids != null && request.Ids.Any())
        {
            query = query.Where(wpu => !request.Ids.Contains(wpu.Id.ToString()));
        }

        return await query.Select(wpu => new WorkUnitResponse
        {
            Id = wpu.Id,
            Name = wpu.Name
        }).ToListAsync();
    }


    // Thêm mới WorkProcess
    public async Task<bool> CreateAsync(WorkProcessCreateRequest request)
    {
        // Kiểm tra xem có AcademicYear phù hợp không
        var startDate = DateTime.Parse(request.StartDate);
        var endDate = DateTime.Parse(request.EndDate);

        var academicYear = await _context.AcademicYears
            .Where(ay => ay.IsDelete == false &&
                         startDate >= ay.StartDate &&
                         endDate <= ay.EndDate)
            .FirstOrDefaultAsync();


        if (academicYear == null)
        {
            throw new ArgumentException("Thời gian bắt đầu và kết thúc phải nằm trong một năm học hợp lệ.");
        }

        // Tạo WorkProcess (EF sẽ kiểm tra khóa ngoại DepartmentId & PositionId)
        var workProcess = new WorkProcess
        {
            UserId = request.UserId,
            OrganizationUnit = request.OrganizationUnit,
            DepartmentId = request.DepartmentId,
            PositionId = request.PositionId,
            StartDate = DateTime.Parse(request.StartDate),
            EndDate = DateTime.Parse(request.EndDate)
        };

        _context.WorkProcesses.Add(workProcess);
        await _context.SaveChangesAsync();

        // Kiểm tra WorkUnitIds hợp lệ
        var workUnitIds = request.WorkUnitIds.Distinct().ToList();
        var validWorkUnits = await _context.WorkUnits
            .Where(wu => workUnitIds.Contains(wu.Id))
            .Select(wu => wu.Id)
            .ToListAsync();

        var invalidWorkUnits = workUnitIds.Except(validWorkUnits).ToList();
        if (invalidWorkUnits.Any())
        {
            throw new ArgumentException($"Các đơn vị công tác không tồn tại: {string.Join(", ", invalidWorkUnits)}.");
        }

        // Thêm WorkProcessUnits
        var workProcessUnits = workUnitIds.Select(workUnitId => new WorkProcessUnit
        {
            WorkProcessId = workProcess.Id,
            WorkUnitId = workUnitId
        }).ToList();

        _context.WorkProcessUnits.AddRange(workProcessUnits);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Có lỗi xảy ra khi tạo quá trình làm việc. Vui lòng kiểm tra dữ liệu đầu vào.", ex);
        }

        return true;
    }


    // Cập nhật WorkProcess
    public async Task<bool> UpdateAsync(WorkProcessUpdateRequest request)
    {
        // Kiểm tra WorkProcess có tồn tại không
        var workProcess = await _context.WorkProcesses.FindAsync(request.Id);
        if (workProcess == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy WorkProcess với ID: {request.Id}.");
        }

        if (workProcess.IsDeleted == true)
        {
            throw new InvalidOperationException($"WorkProcess với ID {request.Id} đã bị xóa trước đó.");
        }

        // Kiểm tra xem có AcademicYear phù hợp không
        var startDate = DateTime.Parse(request.StartDate);
        var endDate = DateTime.Parse(request.EndDate);

        var academicYear = await _context.AcademicYears
            .Where(ay => ay.IsDelete == false &&
                         startDate >= ay.StartDate &&
                         endDate <= ay.EndDate)
            .FirstOrDefaultAsync();

        if (academicYear == null)
        {
            throw new ArgumentException("Thời gian bắt đầu và kết thúc phải nằm trong một năm học hợp lệ.");
        }

        // Cập nhật dữ liệu của WorkProcess
        workProcess.OrganizationUnit = request.OrganizationUnit;
        workProcess.DepartmentId = request.DepartmentId;
        workProcess.PositionId = request.PositionId;
        workProcess.StartDate = DateTime.Parse(request.StartDate);
        workProcess.EndDate = DateTime.Parse(request.EndDate);
        workProcess.UpdatedAt = DateTime.Now;

        // Kiểm tra WorkUnitIds hợp lệ trước khi cập nhật
        var workUnitIds = request.WorkUnitIds.Distinct().ToList();
        var validWorkUnits = await _context.WorkUnits
            .Where(wu => workUnitIds.Contains(wu.Id))
            .Select(wu => wu.Id)
            .ToListAsync();

        var invalidWorkUnits = workUnitIds.Except(validWorkUnits).ToList();
        if (invalidWorkUnits.Any())
        {
            throw new ArgumentException($"Các đơn vị công tác không tồn tại: {string.Join(", ", invalidWorkUnits)}.");
        }

        // Xóa các WorkProcessUnit cũ
        var existingWorkProcessUnits = _context.WorkProcessUnits.Where(wpu => wpu.WorkProcessId == request.Id);
        _context.WorkProcessUnits.RemoveRange(existingWorkProcessUnits);

        // Thêm các WorkProcessUnit mới
        var workProcessUnits = workUnitIds.Select(workUnitId => new WorkProcessUnit
        {
            WorkProcessId = workProcess.Id,
            WorkUnitId = workUnitId
        }).ToList();

        _context.WorkProcessUnits.AddRange(workProcessUnits);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Có lỗi xảy ra khi cập nhật quá trình làm việc. Vui lòng kiểm tra dữ liệu đầu vào.", ex);
        }

        return true;
    }


    public async Task<bool> DeleteAsync(WorkProcessDeleteRequest request)
    {
        // Kiểm tra xem WorkProcess có tồn tại không
        var workProcess = await _context.WorkProcesses.FindAsync(request.Id);
        if (workProcess == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy WorkProcess với ID: {request.Id}.");
        }

        // Nếu đã bị xóa trước đó thì không cần xóa lại
        if (workProcess.IsDeleted == true)
        {
            throw new InvalidOperationException($"WorkProcess với ID {request.Id} đã bị xóa trước đó.");
        }

        // Đánh dấu IsDeleted = true thay vì xóa vật lý
        workProcess.IsDeleted = true;
        workProcess.UpdatedAt = DateTime.Now;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Có lỗi xảy ra khi xóa quá trình làm việc. Vui lòng thử lại.", ex);
        }
    }

}
