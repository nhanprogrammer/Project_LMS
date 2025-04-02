using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface IAcademicYearRepository 
{
    IQueryable<AcademicYear> GetQueryable();

    Task<List<AcademicYear>> GetByIdsAsync(List<int> ids);

    Task UpdateRangeAsync(List<AcademicYear> lessons);

    Task<List<AcademicYear>> SearchAcademicYear(int year);

    Task<bool> IsAcademicYearExist(int academicYearId);
     Task<AcademicYear> GetByIdAsync(int id);
     Task<IEnumerable<AcademicYear>> GetAllAsync();
     Task AddAsync(AcademicYear academicYear);
     Task UpdateAsync(AcademicYear academicYear);
     Task<AcademicYearWithSemestersDto> GetByIdAcademicYearAsync(int id);
}