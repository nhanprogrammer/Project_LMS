using Project_LMS.Models;

namespace Project_LMS.Interfaces.Repositories
{
    public interface ISemesterRepository
    {
        Task<IEnumerable<Semester>> GetAllAsync();
        Task<Semester?> GetByIdAsync(int id);
        Task AddAsync(Semester semester);
        Task UpdateAsync(Semester semester);
        Task DeleteAsync(int id);
    }
}