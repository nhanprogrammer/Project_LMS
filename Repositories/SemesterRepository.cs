using Microsoft.EntityFrameworkCore;
using Project_LMS.Models;
using Project_LMS.Data;
using Project_LMS.Interfaces.Repositories;

namespace Project_LMS.Repositories
{
    public class SemesterRepository : ISemesterRepository
    {
        private readonly ApplicationDbContext _context;

        public SemesterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Semester>> GetAllAsync()
        {
            return await _context.Semesters.ToListAsync();
        }

        public async Task<Semester?> GetByIdAsync(int id)
        {
            return await _context.Semesters.FindAsync(id);
        }

        public async Task AddAsync(Semester semester)
        {
            await _context.Semesters.AddAsync(semester);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Semester semester)
        {
            _context.Semesters.Update(semester);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var semester = await _context.Semesters.FindAsync(id);
            if (semester != null)
            {
                _context.Semesters.Remove(semester);
                await _context.SaveChangesAsync();
            }
        }
    }
}