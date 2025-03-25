using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface IRewardService
    {
        Task<ApiResponse<object>> GetByIdAsync(int id);
        Task<ApiResponse<object>> AddAsync(RewardRequest request);
        Task<ApiResponse<object>> UpdateAsync(UpdateRewardRequest request);
        Task<ApiResponse<object>> DeleteAsync(int id);
    }
}