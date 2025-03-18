using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface IAcademicYearRepository :IRepository<AcademicYear>
{
    IQueryable<AcademicYear> GetQueryable();

    Task<List<AcademicYear>> GetByIdsAsync(List<int> ids);

    Task UpdateRangeAsync(List<AcademicYear> lessons);
}