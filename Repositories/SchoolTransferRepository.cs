using Microsoft.EntityFrameworkCore;
using Project_LMS.Models;
using Project_LMS.Data;
using Project_LMS.Interfaces.Repositories;

namespace Project_LMS.Repositories
{
    public class SchoolTransferRepository : ISchoolTransferRepository
    {
        private readonly ApplicationDbContext _context;

        public SchoolTransferRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SchoolTransfer>> GetAllAsync()
        {
            return await _context.SchoolTransfers.ToListAsync();
        }

        public async Task<SchoolTransfer?> GetByIdAsync(int id)
        {
            return await _context.SchoolTransfers.FindAsync(id);
        }

        public async Task AddAsync(SchoolTransfer schoolTransfer)
        {
            await _context.SchoolTransfers.AddAsync(schoolTransfer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SchoolTransfer schoolTransfer)
        {
            _context.SchoolTransfers.Update(schoolTransfer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var schoolTransfer = await _context.SchoolTransfers.FindAsync(id);
            if (schoolTransfer != null)
            {
                _context.SchoolTransfers.Remove(schoolTransfer);
                await _context.SaveChangesAsync();
            }
        }
    }
}