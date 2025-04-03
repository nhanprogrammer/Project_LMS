using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories
{
    public interface ITeacherClassSubjectRepository
    {
        public Task<TeacherClassSubject> FindSubjectByTeacherIsPrimary(int? teacherId);
        public Task<List<TeacherClassSubject>> GetAllByTeacher(int? teacherId);
        public Task<TeacherClassSubject> AddAsync(TeacherClassSubject teacher);
        public Task DeleteAsync(TeacherClassSubject teacher);
    }
}
