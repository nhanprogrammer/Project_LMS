using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface ISubjectGroupRepository : IRepository<SubjectGroup>
{
    IQueryable<SubjectGroup> GetQueryable();

    

    
    Task SaveAsync();
}
