using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class NationalityRepository : INationalityRepository
    {
        private readonly ApplicationDbContext _context;
        
        public NationalityRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<Nationality>> GetAllAsync()
        {
            return await _context.Nationalities
                .Where(n => n.IsDelete == false || n.IsDelete == null)
                .ToListAsync();
        }

        public async Task<Nationality?> GetByIdAsync(int id)
        {
            return await _context.Nationalities
                .FirstOrDefaultAsync(n => n.Id == id && (n.IsDelete == false || n.IsDelete == null));
        }

        public async Task AddAsync(Nationality nationality)
        {
            nationality.CreateAt = DateTime.UtcNow;
            _context.Nationalities.Add(nationality);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Nationality nationality)
        {
            nationality.UpdateAt = DateTime.UtcNow;
            _context.Nationalities.Update(nationality);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var nationality = await _context.Nationalities.FindAsync(id);
            if (nationality != null)
            {
                nationality.IsDelete = true;
                nationality.UpdateAt = DateTime.UtcNow;
                _context.Nationalities.Update(nationality);
                await _context.SaveChangesAsync();
            }
        }
    }
}