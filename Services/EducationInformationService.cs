using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services;

public class EducationInformationService : IEducationInformationService
{
    private readonly ApplicationDbContext _context;

    public EducationInformationService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Lấy tất cả EducationInformation
    public async Task<IEnumerable<EducationInformationsResponse>> GetAllAsync(EducationInformationRequest request)
    {
        DateTime? startDate = null;
        DateTime? endDate = null;

        // Kiểm tra nếu AcademicYearId hợp lệ thì lấy dữ liệu
        if (request.AcademicYearId > 0)
        {
            var academicYear = await _context.AcademicYears.FindAsync(request.AcademicYearId);
            if (academicYear != null)
            {
                startDate = academicYear.StartDate;
                endDate = academicYear.EndDate;
            }
        }

        // Lấy danh sách TeachingAssignments theo UserId (luôn lấy, không gán rỗng)
        var teachingAssignmentsQuery = _context.TeachingAssignments
            .Where(at => at.UserId == request.UserId);

        // Chỉ lọc theo ClassId nếu ClassId > 0
        if (request.ClassId > 0)
        {
            teachingAssignmentsQuery = teachingAssignmentsQuery.Where(at => at.ClassId == request.ClassId);
        }

        // Lấy dữ liệu TeachingAssignments
        var assignmentsTeaching = await teachingAssignmentsQuery.ToListAsync();

        // Truy vấn EducationInformations
        var query = _context.EducationInformations
            .Where(wp => wp.IsDeleted == false && wp.UserId == request.UserId);

        // Áp dụng điều kiện thời gian nếu có AcademicYearId hợp lệ
        if (startDate.HasValue && endDate.HasValue)
        {
            query = query.Where(wp => wp.StartDate >= startDate.Value && wp.EndDate <= endDate.Value);
        }

        // Nếu có TeachingAssignments và ClassId > 0, sử dụng join để lọc
        if (assignmentsTeaching.Any() && request.ClassId > 0)
        {
            query = from ei in query
                    join ta in teachingAssignmentsQuery
                    on ei.UserId equals ta.UserId
                    where ta.StartDate <= ei.StartDate && ta.EndDate >= ei.EndDate
                    select ei;
        }

        // Áp dụng điều kiện tìm kiếm nếu có Search
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(wp =>
                wp.TrainingInstitution.Contains(request.Search) ||
                wp.Major.Contains(request.Search) ||
                wp.TrainingForm.Contains(request.Search) ||
                wp.CertifiedDegree.Contains(request.Search));
        }

        // Trả về danh sách
        return await query.Select(wp => new EducationInformationsResponse
        {
            Id = wp.Id,
            TrainingInstitution = wp.TrainingInstitution,
            Major = wp.Major,
            StartDate = wp.StartDate,
            EndDate = wp.EndDate,
            TrainingForm = wp.TrainingForm,
            CertifiedDegree = wp.CertifiedDegree
        }).ToListAsync();
    }

    public async Task<EducationInformationResponse> GetById(EducationInformationDeleteRequest request)
    {
        var EducationInformation = await _context.EducationInformations

            .FirstOrDefaultAsync(wp => wp.Id == request.Id && wp.IsDeleted == false);

        if (EducationInformation == null) return null;
        return new EducationInformationResponse
        {
            Id = EducationInformation.Id,
            TrainingInstitution = EducationInformation.TrainingInstitution,
            Major = EducationInformation.Major,
            StartDate = EducationInformation.StartDate,
            EndDate = EducationInformation.EndDate,
            TrainingForm = EducationInformation.TrainingForm,
            CertifiedDegree = EducationInformation.CertifiedDegree,
            AttachedFile = EducationInformation.AttachedFile
        };
    }

    // Lấy tất cả TrainingProgram không bao gồm các TrainingProgram có Id trong chuỗi ids
    public async Task<IEnumerable<TrainingProgramResponse>> GetTrainingProgramsExcluding(TrainingProgramRequest request)
    {
        var query = _context.TrainingPrograms.AsQueryable(); // Khởi tạo query

        // Nếu request.Ids không null và có phần tử thì mới lọc
        if (request.Ids != null && request.Ids.Any())
        {
            query = query.Where(wpu => !request.Ids.Contains(wpu.Id.ToString()));
        }

        return await query.Select(wpu => new TrainingProgramResponse
        {
            Id = wpu.Id,
            Name = wpu.Name
        }).ToListAsync();
    }


    // Thêm mới EducationInformation
    public async Task<bool> CreateAsync(EducationInformationCreateRequest request)
    {

        // Kiểm tra xem có AcademicYear phù hợp không
        var academicYear = await _context.AcademicYears
            .Where(ay => ay.IsDelete == false &&
                         DateTime.Parse(request.StartDate) >= ay.StartDate &&
                         DateTime.Parse(request.EndDate) <= ay.EndDate)
            .FirstOrDefaultAsync();

        if (academicYear == null)
        {
            throw new ArgumentException("Thời gian bắt đầu và kết thúc phải nằm trong một năm học hợp lệ.");
        }

        // Kiểm tra xem User có tồn tại không
        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
        if (!userExists)
        {
            throw new KeyNotFoundException($"Người dùng với ID {request.UserId} không tồn tại.");
        }

        // Tạo EducationInformation (EF Core sẽ tự kiểm tra khóa ngoại TrainingProgramId)
        var educationInformation = new EducationInformation
        {
            UserId = request.UserId,
            TrainingInstitution = request.TrainingInstitution,
            Major = request.Major,
            TrainingForm = request.TrainingForm,
            CertifiedDegree = request.CertifiedDegree,
            AttachedFile = request.AttachedFile,
            StartDate = DateTime.Parse(request.StartDate),
            EndDate = DateTime.Parse(request.EndDate),
        };

        _context.EducationInformations.Add(educationInformation);
        await _context.SaveChangesAsync(); // Lưu trước để có ID

        // Thêm danh sách EducationPrograms (EF Core kiểm tra khóa ngoại tự động)
        var educationPrograms = request.TrainingProgramIds
            .Distinct()
            .Select(tpId => new EducationProgram
            {
                EducationId = educationInformation.Id,
                TrainingProgramId = tpId
            }).ToList();

        _context.EducationPrograms.AddRange(educationPrograms);

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Có lỗi xảy ra khi tạo thông tin đào tạo. Vui lòng kiểm tra dữ liệu đầu vào.", ex);
        }
    }



    // Cập nhật EducationInformation
    public async Task<bool> UpdateAsync(EducationInformationUpdateRequest request)
    {
        // Kiểm tra xem EducationInformation có tồn tại không
        var educationInformation = await _context.EducationInformations.FindAsync(request.Id);
        if (educationInformation == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy EducationInformation với ID: {request.Id}.");
        }

        // Kiểm tra năm học hợp lệ
        var academicYearExists = await _context.AcademicYears
            .AnyAsync(ay => ay.IsDelete == false &&
                            DateTime.Parse(request.StartDate) >= ay.StartDate &&
                            DateTime.Parse(request.EndDate) <= ay.EndDate);
        if (!academicYearExists)
        {
            throw new ArgumentException("Thời gian bắt đầu và kết thúc phải nằm trong một năm học hợp lệ.");
        }

        // Cập nhật thông tin EducationInformation
        educationInformation.TrainingInstitution = request.TrainingInstitution;
        educationInformation.Major = request.Major;
        educationInformation.TrainingForm = request.TrainingForm;
        educationInformation.CertifiedDegree = request.CertifiedDegree;
        educationInformation.AttachedFile = request.AttachedFile;
        educationInformation.StartDate = DateTime.Parse(request.StartDate);
        educationInformation.EndDate = DateTime.Parse(request.EndDate);
        educationInformation.UpdatedAt = DateTime.UtcNow;

        // Xóa các EducationProgram cũ và thêm mới
        var existingPrograms = _context.EducationPrograms
            .Where(ep => ep.EducationId == request.Id);
        _context.EducationPrograms.RemoveRange(existingPrograms);

        var educationPrograms = request.TrainingProgramIds
            .Distinct()
            .Select(trainingProgramId => new EducationProgram
            {
                EducationId = educationInformation.Id,
                TrainingProgramId = trainingProgramId
            }).ToList();

        _context.EducationPrograms.AddRange(educationPrograms);

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Có lỗi xảy ra khi cập nhật thông tin đào tạo. Vui lòng kiểm tra dữ liệu đầu vào.", ex);
        }
    }

    // Xóa mềm EducationInformation
    public async Task<bool> DeleteAsync(EducationInformationDeleteRequest request)
    {
        // Kiểm tra xem EducationInformation có tồn tại không
        var educationInformation = await _context.EducationInformations.FindAsync(request.Id);
        if (educationInformation == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy EducationInformation với ID: {request.Id}.");
        }

        // Nếu đã bị xóa trước đó thì không cần xóa lại
        if (educationInformation.IsDeleted == true)
        {
            throw new InvalidOperationException($"EducationInformation với ID {request.Id} đã bị xóa trước đó.");
        }

        // Đánh dấu IsDeleted = true thay vì xóa vật lý
        educationInformation.IsDeleted = true;
        educationInformation.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Có lỗi xảy ra khi xóa thông tin đào tạo. Vui lòng thử lại.", ex);
        }
    }

}
