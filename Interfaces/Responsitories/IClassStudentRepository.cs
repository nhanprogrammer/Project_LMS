using Project_LMS.DTOs.Request;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories
{
    public interface IClassStudentRepository
    {
        public Task AddAsync(ClassStudentRequest request);
        public Task<List<ClassStudent>> GetAllByClasses(List<int> ids, PaginationRequest request, string column, bool orderBy, string searchTerm);
        public Task<int> CountByClasses(List<int> ids, string searchTerm);

        public Task<List<ClassStudent>> getAllStudentByClasses(List<int> ids);
        public Task<ClassStudent> FindClassStudentByUserCodeClassId(string userCode, int classId);
        public Task<ClassStudent> FindStudentByClassAndStudent(int classId, int studentId);

    }
}
