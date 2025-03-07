using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories
{
    public interface IJwtReponsitory
    {
        string GenerateToken(User user);
        Task<User> AuthenticateAsync(string userName, string password);
        Task<User> RegisterAsync(string username, string password/*, string email*/);
    }
}
