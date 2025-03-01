using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface INationalityRepository
{
    Task<IEnumerable<Nationality>> GetAllAsync();
    Task<Nationality?> GetByIdAsync(int id);
    Task AddAsync(Nationality schoolBranch);
    Task UpdateAsync(Nationality schoolBranch);
    Task DeleteAsync(int id);
}