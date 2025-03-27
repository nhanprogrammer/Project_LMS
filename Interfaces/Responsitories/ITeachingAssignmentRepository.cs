using Project_LMS.Models;
namespace Project_LMS.Interfaces.Responsitories
{
    public interface ITeachingAssignmentRepository
    {
        public Task<List<TeachingAssignment>> GetAllByClasses(List<int> classIds);
    }
}
