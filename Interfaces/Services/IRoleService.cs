using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponse>> GetAllAsync();
        Task<RoleResponse> GetByIdAsync(int id);
    }
}