using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Repositories
{
    public interface ISchoolRepository
    {
        Task<IEnumerable<School>> GetAllAsync();
        Task<School?> GetByIdAsync(int id);
        Task AddAsync(School school);
        Task UpdateAsync(School school);
        Task DeleteAsync(int id);
    }
}