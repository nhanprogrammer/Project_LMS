using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_LMS.Repositories
{
    public class ClassTypeRepository : IClassTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public ClassTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClassType>> GetAllAsync()
        {
            return await _context.ClassTypes.ToListAsync();
        }

        public async Task<ClassType> GetByIdAsync(int id)
        {
            return await _context.ClassTypes
                .FirstOrDefaultAsync(ct => ct.Id == id);
        }

        public async Task AddAsync(ClassType entity)
        {
            await _context.ClassTypes.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ClassType entity)
        {
            _context.ClassTypes.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var classType = await _context.ClassTypes.FindAsync(id);
            if (classType != null)
            {
                _context.ClassTypes.Remove(classType);
                await _context.SaveChangesAsync();
            }
        }
    }
}