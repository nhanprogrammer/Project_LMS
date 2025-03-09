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

    public async Task<ClassStudentOnline> GetByIdAsync(int id)
    {
        return await _context.ClassStudentOnlines.FindAsync(id);
    }

    public async Task<IEnumerable<ClassStudentOnline>> GetAllAsync()
    {
        return await _context.ClassStudentOnlines.ToListAsync();
    }

    public async Task AddAsync(ClassStudentOnline entity)
    {
        await _context.ClassStudentOnlines.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ClassStudentOnline entity)
    {
        _context.ClassStudentOnlines.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.ClassStudentOnlines.FindAsync(id);
        if (entity != null)
        {
            // entity.IsDelete = true;
            _context.ClassStudentOnlines.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}