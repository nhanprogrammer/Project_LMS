using Project_LMS.Models;

namespace Project_LMS.Interfaces.Repositories
{
    public interface IRegistrationRepository
    {
        Task<IEnumerable<Registration>> GetAllAsync();
        Task<Registration?> GetByIdAsync(int id);
        Task AddAsync(Registration registration);
        Task UpdateAsync(Registration registration);
        Task DeleteAsync(int id);
    }
}