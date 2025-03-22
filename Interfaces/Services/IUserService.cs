using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface IUserService
{
    
    public Task<ApiResponse<PaginatedResponse<object>>> GetAll(int pageNumber, int pageSize);
    public Task<ApiResponse<List<UserResponse>>> GetAllByIds(List<int> ids,int pageNumber, int pageSize);
    public Task<ApiResponse<UserResponse>> Create(UserRequest request);
    public Task<ApiResponse<UserResponse>> Update(int id, UserRequest request);
    public Task<ApiResponse<UserResponse>> Delete(int id);
    public Task<ApiResponse<UserResponse>> Search(int id);
    public Task<ApiResponse<object>> ExportUsersToExcel();
    public Task<ApiResponse<object>> CheckUser(string name);
    public Task<ApiResponse<object>> ForgotPassword(ForgotPasswordRequest request);


}