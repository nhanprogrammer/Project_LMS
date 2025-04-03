using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class DependentRepository : IDependentRepository
    {
        private readonly ApplicationDbContext _context;

        public DependentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Dependent> AddAsync(Dependent dependent)
        {
            dependent.CreateAt = DateTime.Now;
            dependent.IsDelete = false;
            await _context.Dependents.AddAsync(dependent);
            await _context.SaveChangesAsync();
            return dependent;
        }

        public async Task DeleteAsync(Dependent dependent)
        {
            dependent.IsDelete = false;
            _context.Dependents.Update(dependent);
            await _context.SaveChangesAsync();
        }

        public async Task<Dependent> FindByIdAsync(int id)
        {
            return await _context.Dependents.FirstOrDefaultAsync(d => d.Id == id && d.IsDelete == false);
        }
    }
}
