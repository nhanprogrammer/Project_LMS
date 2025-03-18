using Project_LMS.Models;

namespace Project_LMS.Interfaces.Repositories
{
    public interface IRewardRepository
    {
        Task<List<Reward>> GetAllAsync();
        Task<Reward?> GetByIdAsync(int id);
        Task AddAsync(Reward reward);
        Task UpdateAsync(Reward reward);
        Task DeleteAsync(int id);
    }
}