using Humanizer.Localisation.DateToOrdinalWords;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories;

public interface IClassRepository : IRepository<Class>
{
    public Task<List<Class>> GetAllClassByAcademicDepartment(int academicId , int departmentId);
}