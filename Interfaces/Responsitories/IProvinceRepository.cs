using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface IProvinceRepository
{
    Task<IEnumerable<Province>> GetAllAsync();
    Task<Province?> GetByIdAsync(int id);
    Task AddAsync(Province province);
    Task UpdateAsync(Province province);
    Task DeleteAsync(int id);
}