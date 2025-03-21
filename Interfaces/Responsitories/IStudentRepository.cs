using Project_LMS.DTOs.Request;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories
{
    public interface IStudentRepository
    {
        public Task<List<User>> GetAll();
        public Task<User> AddAsync(User user);
        public Task<User> UpdateAsync(User user);
        public Task DeleteAsync(User user);
        public Task<List<User>> GetAllOfRewardByIds(bool isReward,List<int> ids,PaginationRequest request, string columnm, bool orderBy, string searchItem);
        public Task<int> CountStudentOfRewardByIds(bool isReward, List<int> ids, string searchItem);
        public Task<User> FindStudentById(int studentId);
        public Task<User> FindStudentByEmailOrderUserCode(string email,string userCode);
        public Task<User> FindStudentByEmail(string email);
        public Task<User> FindStudentByUserCode(string userCode);
        public Task<User> FindStudentByUsername(string username);

        
    }
}
