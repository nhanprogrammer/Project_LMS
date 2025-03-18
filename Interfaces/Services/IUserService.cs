using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface IUserService
{
    
    public Task<ApiResponse<List<UserResponse>>> GetAll(int pageNumber, int pageSize);
    public Task<ApiResponse<List<UserResponse>>> GetAllByIds(List<int> ids,int pageNumber, int pageSize);
    public Task<ApiResponse<UserResponse>> Create(UserRequest request);
    public Task<ApiResponse<UserResponse>> Update(int id, UserRequest request);
    public Task<ApiResponse<UserResponse>> Delete(int id);
    public Task<ApiResponse<UserResponse>> Search(int id);
    
}