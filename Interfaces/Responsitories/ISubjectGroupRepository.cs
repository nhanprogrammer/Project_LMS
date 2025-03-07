using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface ISubjectGroupRepository : IRepository<SubjectsGroup>
{
    IQueryable<SubjectsGroup> GetQueryable();

    

    
    Task SaveAsync();
}
