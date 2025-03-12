using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class StudenStatusRepository : IStudentStatusRepository
    {
        private readonly ApplicationDbContext _context;
        public StudenStatusRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(StudentStatus status)
        {
            await _context.StudentStatuses.AddAsync(status);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(StudentStatus StudentStatus)
        {
            _context.StudentStatuses.Remove(StudentStatus);
            await _context.SaveChangesAsync();
        }

        public async Task<StudentStatus> FindAsync(int id)
        {
            return await _context.StudentStatuses.FindAsync(id);
        }

        public async Task<List<StudentStatus>> GetAllAsync()
        {
            return await _context.StudentStatuses.ToListAsync();
        }

        public async Task UpdateAsync(int id, StudentStatus StudentStatus)
        {
            _context.StudentStatuses.Update(StudentStatus);
            await _context.SaveChangesAsync();
        }
    }

}
