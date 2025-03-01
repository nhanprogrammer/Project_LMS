using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project_LMS.Interfaces.Responsitories;

namespace Project_LMS.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Department> GetByIdAsync(int id)
        {
            return await _context.Departments.FindAsync(id);
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.Departments.Where(ah => (bool)!ah.IsDelete).ToListAsync();
        }

        public async Task AddAsync(Department entity)
        {
            await _context.Departments.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Department entity)
        {
            _context.Departments.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Departments.FindAsync(id);
            if (entity != null)
            {
                entity.IsDelete = true; 
                _context.Departments.Update(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}