using Microsoft.EntityFrameworkCore;
using Project_LMS.Models;
using Project_LMS.Data;
using Project_LMS.Interfaces.Repositories;

namespace Project_LMS.Repositories
{
    public class SchoolRepository : ISchoolRepository
    {
        private readonly ApplicationDbContext _context;

        public SchoolRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<School>> GetAllAsync()
        {
            return await _context.Schools.ToListAsync();
        }

        public async Task<School?> GetByIdAsync(int id)
        {
            return await _context.Schools.FindAsync(id);
        }

        public async Task AddAsync(School school)
        {
            await _context.Schools.AddAsync(school);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(School school)
        {
            _context.Schools.Update(school);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var school = await _context.Schools.FindAsync(id);
            if (school != null)
            {
                _context.Schools.Remove(school);
                await _context.SaveChangesAsync();
            }
        }
    }
}