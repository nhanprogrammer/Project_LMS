using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface IDisciplineRepository 
{
    Task<Discipline?> GetByIdAsync(int id);
    Task<Discipline?> AddAsync(Discipline discipline);
    Task<Discipline> UpdateAsync(Discipline discipline);
    Task DeleteAsync(Discipline discipline);
}