using Project_LMS.Interfaces.Repositories;
using Project_LMS.Models;
using Project_LMS.Data;
using Project_LMS.Exceptions;

namespace Project_LMS.Repositories
{
    public class WardRepository : IWardRepository
    {
        private readonly ApplicationDbContext _context;

        public WardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Ward> GetByIdAsync(int id)
        {
            return await _context.Wards.FindAsync(id) ?? throw new NotFoundException("Ward not found");
        }
    }
}