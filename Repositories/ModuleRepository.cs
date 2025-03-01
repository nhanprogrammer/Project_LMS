using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories;

public class ModuleRepository : IModuleRepository
{
    private readonly ApplicationDbContext _context;

    public ModuleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Module> GetByIdAsync(int id)
    {
        return await _context.Modules.FindAsync(id);
    }

    public async Task<IEnumerable<Module>> GetAllAsync()
    {
        return await _context.Modules.Where(ah => (bool)!ah.IsDelete).ToListAsync();
    }

    public async Task AddAsync(Module entity)
    {
        await _context.Modules.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Module entity)
    {
        _context.Modules.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Modules.FindAsync(id);
        if (entity != null)
        {
            entity.IsDelete = true;
            _context.Modules.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}