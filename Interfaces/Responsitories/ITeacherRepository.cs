
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories
{
    public interface ITeacherRepository
    {
        public Task<User> AddAsync(User request); 
        public Task<User> UpdateAsync(User request); 
        public Task DeleteAsync(User user); 
        public Task<User> FindTeacherByUserCode(string userCode); 
        public Task<User> FindTeacherByEmailOrderUserCode(string email,string userCode); 

        public Task<List<User>> GetAllByIds(List<int> ids, PaginationRequest request,bool orderBy,string column,string search);
        public Task<List<User>> GetAllByIds(List<int> ids, bool orderBy,string column,string search);
        public Task<int> CountByClasses(List<int> ids,string search);
        Task<List<UserResponseTeachingAssignment>> GetTeachersAsync();


    }
}
