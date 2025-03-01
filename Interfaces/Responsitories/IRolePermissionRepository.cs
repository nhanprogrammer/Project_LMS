using Project_LMS.Models;

namespace Project_LMS.Interfaces.Repositories
{
    public interface IRolePermissionRepository
    {
        Task<IEnumerable<RolePermission>> GetAllAsync();
        Task<RolePermission?> GetByIdAsync(int id);
        Task AddAsync(RolePermission rolePermission);
        Task UpdateAsync(RolePermission rolePermission);
        Task DeleteAsync(int id);
    }
}