using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories;

public class ClassStudentOnlineRepository : IClassStudentOnlineRepository
{
    private readonly ApplicationDbContext _context;

    public ClassStudentOnlineRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ClassStudentsOnline> GetByIdAsync(int id)
    {
        return await _context.ClassStudentsOnlines.FindAsync(id);
    }

    public async Task<IEnumerable<ClassStudentsOnline>> GetAllAsync()
    {
        return await _context.ClassStudentsOnlines.ToListAsync();
    }

    public async Task AddAsync(ClassStudentsOnline entity)
    {
        await _context.ClassStudentsOnlines.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ClassStudentsOnline entity)
    {
        _context.ClassStudentsOnlines.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.ClassStudentsOnlines.FindAsync(id);
        if (entity != null)
        {
            // entity.IsDelete = true;
            _context.ClassStudentsOnlines.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}