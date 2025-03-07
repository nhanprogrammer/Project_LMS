// Interfaces/Repositories/ISubjectsGroupRepository.cs
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Repositories
{
    public interface ISubjectsGroupRepository
    {
        Task<IEnumerable<SubjectsGroup>> GetAll(int pageNumber, int pageSize);
        Task<SubjectsGroup> GetById(int id);
        Task<SubjectsGroup> Add(SubjectsGroup subjectsGroup);
        Task<SubjectsGroup> Update(int id, SubjectsGroup subjectsGroup);
        Task<bool> Delete(int id);
    }
}