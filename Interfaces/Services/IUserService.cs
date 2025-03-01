using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface IUserService
{
    
    Task<ApiResponse<List<UserResponse>>> GetAll();
    Task<ApiResponse<UserResponse>> Create(UserRequest request);
    Task<ApiResponse<UserResponse>> Update(int id, UserRequest request);
    Task<ApiResponse<UserResponse>> Delete(int id);
    Task<ApiResponse<UserResponse>> Search(int id);
    
}