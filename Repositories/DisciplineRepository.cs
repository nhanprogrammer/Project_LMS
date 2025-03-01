using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories;

public class DisciplineRepository : IDisciplineRepository
{
    private readonly ApplicationDbContext _context;

    public DisciplineRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Discipline> GetByIdAsync(int id)
    {
        return await _context.Disciplines.FindAsync(id);
    }

    public async Task<IEnumerable<Discipline>> GetAllAsync()
    {
        return await _context.Disciplines.Where(ah => (bool)!ah.IsDelete).ToListAsync();
    }

    public async Task AddAsync(Discipline entity)
    {
        await _context.Disciplines.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Discipline entity)
    {
        _context.Disciplines.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Disciplines.FindAsync(id);
        if (entity != null)
        {
            entity.IsDelete = true;
            _context.Disciplines.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}