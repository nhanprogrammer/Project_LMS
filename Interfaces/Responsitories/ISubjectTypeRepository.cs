using Project_LMS.Models;

namespace Project_LMS.Interfaces.Repositories
{
    public interface ISubjectTypeRepository
    {
        Task<IEnumerable<SubjectType>> GetAll(int pageNumber, int pageSize);
        Task<SubjectType?> GetById(int id);
        Task<SubjectType> Add(SubjectType subjectType);
        Task<SubjectType?> Update(int id, SubjectType subjectType);
        Task<bool> Delete(int id);
    }
}

