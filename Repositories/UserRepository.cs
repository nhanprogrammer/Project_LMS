using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> FindAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Id == id && (bool)user.IsDelete == false);
        }

        public async Task<List<User>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Users
                .Include(user =>user.Role)
                .Include(user =>user.StudentStatus)
                .Where(user => user.IsDelete == false && user.Role.Name.Equals("Student"))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public Task<List<User>> GetAllByIdsAsync(List<int> ids, int pageNumber, int pageSize)
        {
            return _context.Users.Where(u => ids.Contains(u.Id))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task UpdateAsync( User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
