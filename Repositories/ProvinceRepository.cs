using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class ProvinceRepository : IProvinceRepository
    {
        private readonly ApplicationDbContext _context;
        
        public ProvinceRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<Province>> GetAllAsync()
        {
            return await _context.Provinces
                .Where(p => p.IsDelete == false || p.IsDelete == null)
                .Include(p => p.Districts)
                .Include(p => p.SchoolTransfers)
                .ToListAsync();
        }

        public async Task<Province?> GetByIdAsync(int id)
        {
            return await _context.Provinces
                .Include(p => p.Districts)
                .Include(p => p.SchoolTransfers)
                .FirstOrDefaultAsync(p => p.Id == id && (p.IsDelete == false || p.IsDelete == null));
        }

        public async Task AddAsync(Province province)
        {
            province.CreateAt = DateTime.UtcNow;
            _context.Provinces.Add(province);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Province province)
        {
            province.UpdateAt = DateTime.UtcNow;
            _context.Provinces.Update(province);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var province = await _context.Provinces.FindAsync(id);
            if (province != null)
            {
                province.IsDelete = true;
                province.UpdateAt = DateTime.UtcNow;
                _context.Provinces.Update(province);
                await _context.SaveChangesAsync();
            }
        }
    }
}