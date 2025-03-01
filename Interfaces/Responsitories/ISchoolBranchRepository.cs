using Project_LMS.Models;

namespace Project_LMS.Interfaces.Repositories
{
    public interface ISchoolBranchRepository
    {
        Task<IEnumerable<SchoolBranch>> GetAllAsync();
        Task<SchoolBranch?> GetByIdAsync(int id);
        Task AddAsync(SchoolBranch schoolBranch);
        Task UpdateAsync(SchoolBranch schoolBranch);
        Task DeleteAsync(int id);
    }
}