using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface IAcademicHoldRepository  : IRepository<AcademicHold>
{
    Task<AcademicHold> GetByStudentIdAsync(int id);
}