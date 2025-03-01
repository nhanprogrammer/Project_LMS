using Project_LMS.Models;

namespace Project_LMS.Interfaces.Repositories
{
    public interface ISchoolTransferRepository
    {
        Task<IEnumerable<SchoolTransfer>> GetAllAsync();
        Task<SchoolTransfer?> GetByIdAsync(int id);
        Task AddAsync(SchoolTransfer schoolTransfer);
        Task UpdateAsync(SchoolTransfer schoolTransfer);
        Task DeleteAsync(int id);
    }
}