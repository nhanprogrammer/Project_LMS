using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface IAcademicHoldRepository : IRepository<AcademicHold>
{
    Task<AcademicHold> GetByStudentIdAsync(int id);
    //Task<IEnumerable<AcademicHold>> GetAllAsync();
    //Task AddAsync(AcademicHold question);
    //Task UpdateAsync(AcademicHold question);
    //Task DeleteAsync(int id);
    IQueryable<AcademicHold> GetQueryable();
    


}