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
        return await _context.Disciplines.Where(d => d.Id == id && d.IsDelete == false && d.User.IsDelete == false).FirstOrDefaultAsync();
    }

    public async Task<Discipline> AddAsync(Discipline entity)
    {
        await _context.Disciplines.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Discipline> UpdateAsync(Discipline entity)
    {
        _context.Disciplines.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Discipline discipline)
    {
        discipline.IsDelete = true;
        _context.Disciplines.Update(discipline);
        await _context.SaveChangesAsync();
    }
}