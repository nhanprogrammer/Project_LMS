using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface IDepartmentRepository : IRepository<Department>
{
    public Task<Department> GetByIdAsync(int id);
    public Task<IEnumerable<Department>> GetAllAsync();
    public Task AddAsync(Department entity);
    public Task UpdateAsync(Department entity);
    public Task DeleteAsync(int id);
    Task<Department> GetByNameAsync(string name);
}