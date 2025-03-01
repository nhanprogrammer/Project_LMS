using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface IUserTrainingRankService
{
    Task<ApiResponse<List<UserTrainingRankResponse>>> GetAll();
    Task<ApiResponse<UserTrainingRankResponse>> Create(UserTrainingRankRequest request);
    Task<ApiResponse<UserTrainingRankResponse>> Update(int id, UserTrainingRankRequest request);
    Task<ApiResponse<UserTrainingRankResponse>> Delete(int id);
    Task<ApiResponse<UserTrainingRankResponse>> Search(int id);
    
}