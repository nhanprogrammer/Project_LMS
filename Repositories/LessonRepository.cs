using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly ApplicationDbContext _context;

    public LessonRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Lesson> GetByIdAsync(int id)
    {
        return await _context.Lessons.FindAsync(id);
    }

    public async Task<IEnumerable<Lesson>> GetAllAsync()
    {
        return await _context.Lessons.Where(ah => (bool)!ah.IsDelete).ToListAsync();
    }

    public async Task AddAsync(Lesson entity)
    {
        await _context.Lessons.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Lesson entity)
    {
        _context.Lessons.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Lessons.FindAsync(id);
        if (entity != null)
        {
            entity.IsDelete = true;
            _context.Lessons.Update(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Lesson>> GetByIdsAsync(List<int> ids)
    {
        return await _context.Lessons.Where(l => ids.Contains(l.Id)).ToListAsync();
    }

    public async Task UpdateRangeAsync(List<Lesson> lessons)
    {
        _context.Lessons.UpdateRange(lessons);
        await _context.SaveChangesAsync();
    }

    public Task<IQueryable<Lesson>> GetQueryable()
    {
        return Task.FromResult(_context.Lessons.AsQueryable());
    }
}