using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface IRewardService
    {
        Task<IEnumerable<RewardResponse>> GetAllAsync();
        Task<RewardResponse> GetByIdAsync(int id);
        Task<RewardResponse> CreateAsync(RewardRequest request);
        Task<RewardResponse> UpdateAsync(int id, RewardRequest request);
        Task<RewardResponse> DeleteAsync(int id);
    }
}