using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories
{
    public interface IStudentStatusRepository
    {
        public Task<List<StudentStatus>> GetAllAsync();
        public Task AddAsync(StudentStatus status);
        public Task UpdateAsync(int id, StudentStatus status);
        public Task DeleteAsync(StudentStatus status);
        public Task<StudentStatus> FindAsync(int id);
    }
}
