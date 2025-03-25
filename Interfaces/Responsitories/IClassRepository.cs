
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface IClassRepository : IRepository<Class>
{
    public Task<List<Class>> GetAllClassByAcademicDepartment(int academicId , int departmentId);
    public Task<List<Class>> GetAllClassByAcademic(int academicId );
    public Task<Class> FindClassById(int id);
}