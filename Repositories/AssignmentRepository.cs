using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly ApplicationDbContext _context;

    public AssignmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Assignment> GetByIdAsync(int id)
    {
        return await _context.Assignments.FindAsync(id);
    }

    public async Task<IEnumerable<Assignment>> GetAllAsync()
    {
        return await _context.Assignments.ToListAsync();
    }

    public async Task AddAsync(Assignment entity)
    {
        await _context.Assignments.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Assignment entity)
    {
        _context.Assignments.Update(entity);
        await _context.SaveChangesAsync();

    }

    public async Task DeleteAsync(int id)
    {
        var assignment = await _context.Assignments.FindAsync(id);
        if (assignment != null)
        {
            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();
        }
    }
}