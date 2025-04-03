using Project_LMS.Models;

namespace Project_LMS.Interfaces.Repositories
{
    public interface IRewardRepository
    {
        Task<Reward?> GetByIdAsync(int id);
        Task<Reward?> AddAsync(Reward reward);
        Task<Reward> UpdateAsync(Reward reward);
        Task DeleteAsync(Reward reward);
    }
}