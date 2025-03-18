using Project_LMS.DTOs.Request;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories
{
    public interface IUserRepository
    {
        public Task<List<User>> GetAllAsync(int pageNumber, int pageSize);
        public Task AddAsync(User user);
        public Task UpdateAsync( User user);
        public Task DeleteAsync(User user);
        public Task<User> FindAsync(int id);
        public Task<List<User>> GetAllByIdsAsync(List<int> ids, int pageNumber, int pageSize);
    }
}
