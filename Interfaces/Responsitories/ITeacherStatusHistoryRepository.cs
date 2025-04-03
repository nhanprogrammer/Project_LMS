using Project_LMS.Models;
namespace Project_LMS.Interfaces.Responsitories
{
    public interface ITeacherStatusHistoryRepository
    {
        public Task<TeacherStatusHistory> AddAsync(TeacherStatusHistory teacher, string statusName);
        public Task<TeacherStatusHistory> Update(TeacherStatusHistory teacher);
        public Task Delete(TeacherStatusHistory teacher);
        public Task<TeacherStatusHistory> GetByTeacher(int teacherId);
    }
}
