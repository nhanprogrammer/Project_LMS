using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface IRolePermissionService
    {
        Task<IEnumerable<RolePermissionResponse>> GetAllAsync();
        Task<RolePermissionResponse> GetByIdAsync(int id);
        Task<RolePermissionResponse> CreateAsync(RolePermissionRequest request);
        Task<RolePermissionResponse> UpdateAsync(int id, RolePermissionRequest request);
        Task<RolePermissionResponse> DeleteAsync(int id);
    }
}