using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface IPermissionRepository
{
    Task<IEnumerable<Permission>> GetAllAsync();
    Task<Permission?> GetByIdAsync(int id);
    Task AddAsync(Permission permission);
    Task UpdateAsync(Permission permission);
    Task DeleteAsync(int id);
}