using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface IWardService
{

    Task<ApiResponse<List<WardResponse>>> GetAll();
    Task<ApiResponse<WardResponse>> Create(WardRequest request);
    Task<ApiResponse<WardResponse>> Update(int id, WardRequest request);
    Task<ApiResponse<WardResponse>> Delete(int id);
    Task<ApiResponse<WardResponse>> Search(int id);
    
}