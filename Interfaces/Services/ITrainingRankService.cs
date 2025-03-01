using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ITrainingRankService
{
    Task<ApiResponse<List<TrainingRankResponse>>> GetAll();
    Task<ApiResponse<TrainingRankResponse>> Create(TrainingRankRequest request);
    Task<ApiResponse<TrainingRankResponse>> Update(int id, TrainingRankRequest request);
    Task<ApiResponse<TrainingRankResponse>> Delete(int id);
    Task<ApiResponse<TrainingRankResponse>> Search(int id);
    
}