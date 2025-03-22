// Interfaces/Repositories/ISubjectGroupRepository.cs
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Repositories
{
    public interface ISubjectGroupRepository
    {
        Task<SubjectGroup> GetByIdAsync(int id);



        Task SaveAsync();


        Task<IEnumerable<SubjectGroup>> GetAllAsync();


        Task AddAsync(SubjectGroup entity);


        Task UpdateAsync(SubjectGroup entity);


        Task DeleteAsync(int id);


        IQueryable<SubjectGroup> GetQueryable();


    }
}