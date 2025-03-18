using Project_LMS.DTOs.Request;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories
{
    public interface IStudentRepository
    {
        public Task<User> AddAsync(User user);
        public Task<User> UpdateAsync(User user);
        public Task<User> FindById(int id);
        public Task<List<User>> GetAllOfRewardByIds(bool isReward,List<int> ids,PaginationRequest request, string columnm, bool orderBy, string searchItem);
        public Task<int> CountStudentOfRewardByIds(bool isReward, List<int> ids, string searchItem);
        public Task<User> FindStudentById(int studentId);
    }
}
