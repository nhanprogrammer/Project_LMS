using Project_LMS.DTOs.Request;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories
{
    public interface IClassStudentRepository
    {
        public Task AddAsync(ClassStudentRequest request);
        public Task UpdateClassIdAsync(int studentId, int classId);
        public Task<List<ClassStudent>> GetAllByClasses(List<int> ids, PaginationRequest request, string column, bool orderBy, string searchTerm);
        public Task<int> CountByClasses(List<int> ids, string searchTerm);

        public Task<List<ClassStudent>> getAllStudentByClasses(List<int> ids);
        public Task<ClassStudent> FindClassStudentByUserCodeClassId(string userCode, int classId);
        public Task<ClassStudent> FindStudentByClassAndStudent(int classId, int studentId);
        public Task<ClassStudent> FindStudentByIdIsActive(int studentId);
        public Task<List<ClassStudent>> FindStudentByStudentAcademic(int studentId, int academicId);
        Task<ClassStudent?> FindByUserId(int userId);
        public Task<List<ClassStudent>> FindAllClassStudentByUserId(int userId);
        Task<ClassStudent?> FindByUserIdAndSchoolYearAndClassId(int userId, int schoolYearId, int classId);
        Task UpdateAsync(ClassStudent classStudent);
        Task<ClassStudent> FindByUserIdAndSchoolYear(int userId, int schoolYear);
        public Task AddChangeClassAsync(ClassStudentRequest request);
        public Task<ClassStudent> GetClassStudentChangeInfo(int userId, int classId);
        public Task<List<ClassStudent>> FindAllStudentByIdIsActive(int studentId);
        public Task<IEnumerable<ClassStudent>> FindStudentByStudentDepartment(int studentId, int departmentId);
    }
}
