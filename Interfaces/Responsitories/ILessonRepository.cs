using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface ILessonRepository : IRepository<Lesson>
{
    Task<IQueryable<Lesson>> GetQueryable();
}