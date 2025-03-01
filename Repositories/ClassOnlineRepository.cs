using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories;

public class ClassOnlineRepository : IClassOnlineRepository
{
    private readonly ApplicationDbContext _context;

    public ClassOnlineRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ClassOnline> GetByIdAsync(int id)
    {
        return await _context.ClassOnlines.FindAsync(id);
    }

    public async Task<IEnumerable<ClassOnline>> GetAllAsync()
    {
        return await _context.ClassOnlines.Where(ah => (bool)!ah.IsDelete).ToListAsync();
    }

    public async Task AddAsync(ClassOnline entity)
    {
        await _context.ClassOnlines.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ClassOnline entity)
    {
        _context.ClassOnlines.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.ClassOnlines.FindAsync(id);
        if (entity != null)
        {
            entity.IsDelete = true; 
            _context.ClassOnlines.Update(entity);  
            await _context.SaveChangesAsync();
        }
    }
}