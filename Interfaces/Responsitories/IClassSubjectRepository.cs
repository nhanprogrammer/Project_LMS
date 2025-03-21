using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories
{
    public interface IClassSubjectRepository
    {
        public Task<List<ClassSubject>> GetAllByClass(int classId);
    }
}
