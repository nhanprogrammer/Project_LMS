using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories;

public class AnswerRepository : IAnswerRepository
{
    private readonly ApplicationDbContext _context;

    public AnswerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Answer>> GetAllAsync()
    {
        return await _context.Answers.ToListAsync();
    }

    public async Task<Answer> GetByIdAsync(int id)
    {
        return await _context.Answers.FindAsync(id);
    }

    public async Task AddAsync(Answer entity)
    {
        await _context.Answers.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Answer entity)
    {
        _context.Answers.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var answer = await _context.Answers.FindAsync(id);
        if (answer != null)
        {
            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();
        }
    }
}