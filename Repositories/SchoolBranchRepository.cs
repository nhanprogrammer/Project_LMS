using Microsoft.EntityFrameworkCore;
using Project_LMS.Models;
using Project_LMS.Data;
using Project_LMS.Interfaces.Repositories;

namespace Project_LMS.Repositories
{
    public class SchoolBranchRepository : ISchoolBranchRepository
    {
        private readonly ApplicationDbContext _context;

        public SchoolBranchRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SchoolBranch>> GetAllAsync()
        {
            return await _context.SchoolBranches.ToListAsync();
        }

        public async Task<SchoolBranch?> GetByIdAsync(int id)
        {
            return await _context.SchoolBranches.FindAsync(id);
        }

        public async Task AddAsync(SchoolBranch schoolBranch)
        {
            await _context.SchoolBranches.AddAsync(schoolBranch);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SchoolBranch schoolBranch)
        {
            _context.SchoolBranches.Update(schoolBranch);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var schoolBranch = await _context.SchoolBranches.FindAsync(id);
            if (schoolBranch != null)
            {
                _context.SchoolBranches.Remove(schoolBranch);
                await _context.SaveChangesAsync();
            }
        }
    }
}