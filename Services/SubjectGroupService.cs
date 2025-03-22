using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Mappers;
using Project_LMS.Models;

namespace Project_LMS.Services;

public class SubjectGroupService : ISubjectGroupService
{
    private readonly ISubjectGroupRepository _subjectGroupRepository;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public SubjectGroupService(ISubjectGroupRepository subjectGroupRepository, ApplicationDbContext context,
        IMapper mapper)
    {
        _subjectGroupRepository = subjectGroupRepository;
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<SubjectGroupResponse>> GetSubjectGroupById(int id)
    {

        var subjectGroups = await _subjectGroupRepository.GetByIdAsync(id);
        
        if (subjectGroups == null)
        {
            return new ApiResponse<SubjectGroupResponse>(1, "Nhóm môn học không tồn tại", null);
        }
        var subjectGroupResponses = _mapper.Map<SubjectGroupResponse>(subjectGroups);

        return new ApiResponse<SubjectGroupResponse>(0, "Lấy dữ liệu thành công", subjectGroupResponses);
    }

   public async Task<ApiResponse<PaginatedResponse<SubjectGroupResponse>>> GetAllSubjectGroupAsync(
    int? pageNumber,
    int? pageSize,
    string? sortDirection // "asc" hoặc "desc"
)
{
    // 0. Kiểm tra dữ liệu đầu vào
    if (pageNumber.HasValue && pageNumber <= 0)
    {
        return new ApiResponse<PaginatedResponse<SubjectGroupResponse>>(
            1,
            "Giá trị pageNumber phải lớn hơn 0",
            null
        );
    }

    if (pageSize.HasValue && pageSize <= 0)
    {
        return new ApiResponse<PaginatedResponse<SubjectGroupResponse>>(
            1,
            "Giá trị pageSize phải lớn hơn 0",
            null
        );
    }

    if (!string.IsNullOrEmpty(sortDirection) &&
        !sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase) &&
        !sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
    {
        return new ApiResponse<PaginatedResponse<SubjectGroupResponse>>(
            1,
            "Giá trị sortDirection phải là 'asc' hoặc 'desc'",
            null
        );
    }

    try
    {
        // 1. Xác định pageNumber, pageSize mặc định
        var currentPage = pageNumber ?? 1;
        var currentPageSize = pageSize ?? 10;

        // 2. Lấy danh sách subject groups
        var subjectGroups = await _subjectGroupRepository.GetAllAsync();
        var subjectGroupQuery = subjectGroups.AsQueryable();

        // 3. Nếu không nhập sortDirection, mặc định là "asc"
        sortDirection ??= "asc";

        subjectGroupQuery = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
            ? subjectGroupQuery.OrderByDescending(sg => sg.Name).ThenByDescending(sg => sg.Id)
            : subjectGroupQuery.OrderBy(sg => sg.Name).ThenByDescending(sg => sg.Id);

        // 5. Tính toán tổng số dòng, số trang
        var totalItems = subjectGroupQuery.Count();
        var totalPages = (int)Math.Ceiling((double)totalItems / currentPageSize);

        // 6. Phân trang
        var pagedSubjectGroups = subjectGroupQuery
            .Skip((currentPage - 1) * currentPageSize)
            .Take(currentPageSize)
            .ToList();

        // 7. Map sang DTO
        var mappedData = _mapper.Map<List<SubjectGroupResponse>>(pagedSubjectGroups);

        // 8. Tạo đối tượng phân trang
        var paginatedResponse = new PaginatedResponse<SubjectGroupResponse>
        {
            Items = mappedData,
            PageNumber = currentPage,
            PageSize = currentPageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasPreviousPage = currentPage > 1,
            HasNextPage = currentPage < totalPages
        };

        // 9. Trả về
        return new ApiResponse<PaginatedResponse<SubjectGroupResponse>>(
            0,
            "Lấy dữ liệu thành công",
            paginatedResponse
        );
    }
    catch (Exception ex)
    {
        return new ApiResponse<PaginatedResponse<SubjectGroupResponse>>(
            1,
            $"Lỗi: {ex.Message}",
            null
        );
    }
}



    public async Task<ApiResponse<SubjectGroupResponse>> CreateSubjectGroupAsync(
        CreateSubjectGroupRequest createSubjectGroupRequest)
    {
        var subjectGroup = _mapper.Map<SubjectGroup>(createSubjectGroupRequest);

        await _subjectGroupRepository.AddAsync(subjectGroup);

        var createdSubjectGroup = await _subjectGroupRepository
            .GetQueryable()
            .Where(sg => sg.Id == subjectGroup.Id)
            .Include(sg => sg.User)
            .Include(sg => sg.SubjectGroupSubjects)
            .ThenInclude(sgs => sgs.Subject)
            .FirstOrDefaultAsync();
        var subjectGroupResponse = _mapper.Map<SubjectGroupResponse>(createdSubjectGroup);
        return new ApiResponse<SubjectGroupResponse>
        (
            0, "Fill dữ liệu thành công", subjectGroupResponse
        );
    }

public async Task<ApiResponse<SubjectGroupResponse>> UpdateSubjectGroupAsync( UpdateSubjectGroupRequest updateSubjectGroupRequest)
{
    var subjectGroupId = updateSubjectGroupRequest.Id;
    var subjectGroup = await _subjectGroupRepository.GetByIdAsync(subjectGroupId);

    if (subjectGroup == null)
    {
        return new ApiResponse<SubjectGroupResponse>(1, "Nhóm môn học không tồn tại", null);
    }

    
    subjectGroup.Name = updateSubjectGroupRequest.Name;
    subjectGroup.UserId = updateSubjectGroupRequest.UserId;
    subjectGroup.UpdateAt = DateTime.UtcNow.ToLocalTime();

    var subjectIds = updateSubjectGroupRequest.SubjectIds;
    var currentSubjectIds = subjectGroup.SubjectGroupSubjects.Select(sgs => sgs.SubjectId).ToList();

    
    Console.WriteLine($"subjectIds từ request: {string.Join(", ", subjectIds)}");
    Console.WriteLine($"currentSubjectIds từ cơ sở dữ liệu: {string.Join(", ", currentSubjectIds)}");

    
    var subjectsToAdd = subjectIds
        .Except(currentSubjectIds) 
        .Select(subjectId => new SubjectGroupSubject
        {
            SubjectGroupId = subjectGroup.Id,
            SubjectId = subjectId
        }).ToList();

    var validSubjectsToAdd = new List<SubjectGroupSubject>();
    foreach (var subject in subjectsToAdd)
    {
        var existingSubjectGroupSubject = await _context.SubjectGroupSubjects
            .FirstOrDefaultAsync(sgs => sgs.SubjectGroupId == subject.SubjectGroupId && sgs.SubjectId == subject.SubjectId);

        if (existingSubjectGroupSubject == null)
        {
            validSubjectsToAdd.Add(subject);
        }
    }
    var subjectsToRemove = subjectGroup.SubjectGroupSubjects
        .Where(sgs => !subjectIds.Contains(sgs.SubjectId)) 
        .ToList();
    Console.WriteLine($"Sẽ xóa {subjectsToRemove.Count} môn học:");
    foreach (var subjectToRemove in subjectsToRemove)
    {
        Console.WriteLine($"Môn học cần xóa: SubjectId = {subjectToRemove.SubjectId}, SubjectGroupId = {subjectToRemove.SubjectGroupId}");
    }
    if (validSubjectsToAdd.Any())
    {
        subjectGroup.SubjectGroupSubjects.AddRange(validSubjectsToAdd);
    }
    if (subjectsToRemove.Any())
    {
        _context.SubjectGroupSubjects.RemoveRange(subjectsToRemove);
    }
    await _context.SaveChangesAsync();
    var updatedSubjectGroup = await _subjectGroupRepository
        .GetQueryable()
        .Where(sg => sg.Id == subjectGroup.Id)
        .Include(sg => sg.User)
        .Include(sg => sg.SubjectGroupSubjects)
        .ThenInclude(sgs => sgs.Subject)
        .FirstOrDefaultAsync();

    if (updatedSubjectGroup == null)
    {
        return new ApiResponse<SubjectGroupResponse>(1, "Cập nhật nhóm môn học thất bại", null);
    }

    var subjectGroupResponse = new SubjectGroupResponse
    {
        Name = updatedSubjectGroup.Name,
        FullName = updatedSubjectGroup.User.FullName,
        Subjects = updatedSubjectGroup.SubjectGroupSubjects.Select(sgs => new SubjectInfo
        {
            Id = sgs.SubjectId,
            SubjectCode = sgs.Subject.SubjectCode,
            SubjectName = sgs.Subject.SubjectName
        }).ToList()
    };

    return new ApiResponse<SubjectGroupResponse>(0, "Cập nhật dữ liệu thành công", subjectGroupResponse);
}




public async Task<ApiResponse<SubjectGroupResponse>> DeleteSubjectGroupAsync(int id)
{
    if (id <= 0)
    {
        return new ApiResponse<SubjectGroupResponse>(1, "ID không hợp lệ", null);
    }

    await _subjectGroupRepository.DeleteAsync(id);
    return new ApiResponse<SubjectGroupResponse>(0, "Xóa dữ liệu thành công", null);
}


    public async Task<ApiResponse<SubjectGroupResponse>> DeleteSubjectGroupSubject(DeleteRequest deleteRequest)
    {
        var subjectsToRemove = await _context.SubjectGroupSubjects
            .Where(sgs => deleteRequest.ids.Contains(sgs.Id))
            .ToListAsync();

        if (!subjectsToRemove.Any())
        {
            return new ApiResponse<SubjectGroupResponse>(1, "Không tìm thấy môn học để xóa", null);
        }

        _context.SubjectGroupSubjects.RemoveRange(subjectsToRemove);

        return new ApiResponse<SubjectGroupResponse>(0, "Xóa môn học thành công", null);
    }

}